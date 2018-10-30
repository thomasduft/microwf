import { Observable, Subscription } from 'rxjs';

import { Component, OnInit, HostBinding, ViewChild } from '@angular/core';

import { AutoUnsubscribe } from './../shared/services/autoUnsubscribe';
import { ListComponent } from '../shared/list/list.component';

import {
  WorkflowService,
  Workflow,
  WorkflowSearchModel,
  WorkflowPagingModel
} from './../workflow/index';
import { PagingModel } from '../shared/services/models';

@AutoUnsubscribe
@Component({
  selector: 'tw-admin-dashboard',
  providers: [WorkflowService],
  template: `
  <div class="pane__left" twScroller (scrollEnd)="loadNextPage()">
    <tw-list #list [rows]="workflows">
      <tw-header>
        <h3 i18n>Instances</h3>
        <div>
          <button type="button" (click)="reload()">
            <tw-icon name="refresh"></tw-icon>
          </button>
          <tw-workflow-search
            (searchClicked)="searchClicked($event)">
          </tw-workflow-search>
        </div>
      </tw-header>
      <ng-template let-workflow twTemplate="workflow-item">
        <tw-workflow-list-item [workflow]="workflow"></tw-workflow-list-item>
      </ng-template>
    </tw-list>
  </div>
  <div class="pane__main">
    <router-outlet></router-outlet>
  </div>`
})
export class AdminDashboardComponent implements OnInit {
  private workflows$: Subscription;
  private page: PagingModel = PagingModel.create();
  private searchModel: WorkflowSearchModel;

  public workflows: Array<Workflow> = [];

  @HostBinding('class')
  public workspace = 'pane';

  @ViewChild(ListComponent)
  public list: ListComponent;

  public constructor(
    private _service: WorkflowService
  ) { }

  public ngOnInit(): void {
    const model = this.createModel(this.page, this.searchModel);
    this.loadPage(model);
  }

  public reload(): void {
    this.list.rows = [];

    this.ngOnInit();
  }

  public loadNextPage(): void {
    if (this.page.totalPages - 1 > this.page.pageIndex) {
      const model = this.createModel(
        PagingModel.createNextPage(this.page.pageIndex),
        this.searchModel
      );
      this.loadPage(model);
    }
  }

  public searchClicked(searchModel: WorkflowSearchModel): void {
    this.searchModel = searchModel;
    this.list.rows = [];

    this.page = PagingModel.create();
    const model = this.createModel(this.page, this.searchModel);
    this.loadPage(model);
  }

  private loadPage(workflowPagingModel: WorkflowPagingModel): void {
    this.workflows$ = this._service.workflows(workflowPagingModel)
      .subscribe((response: any) => {
        const xPagination = JSON.parse(response.headers.get('X-Pagination'));
        this.page = PagingModel.fromResponse(xPagination);

        this.list.attachRows(response.body);
      });
  }

  private createModel(
    page: PagingModel,
    searchModel?: WorkflowSearchModel
  ): WorkflowPagingModel {
    const model = WorkflowPagingModel.createWorkflowPagingModel(page, searchModel);
    return model;
  }
}
