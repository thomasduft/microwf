import { Observable, Subscription } from 'rxjs';

import { Component, OnInit, HostBinding, ViewChild } from '@angular/core';

import { AutoUnsubscribe } from './../shared/services/autoUnsubscribe';
import { ListComponent } from '../shared/list/list.component';

import { WorkflowService, Workflow } from './../workflow/index';
import { PagingnModel } from '../shared/services/models';

@AutoUnsubscribe
@Component({
  selector: 'tw-admin-dashboard',
  providers: [WorkflowService],
  template: `
  <div class="pane__left" twScroller (scrollEnd)="loadNextPage()">
    <tw-list #list [rows]="workflows">
      <tw-header>
        <h3 i18n>Instances</h3>
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
  private _workflows$: Subscription;
  private _page: PagingnModel = PagingnModel.create();

  public workflows: Array<Workflow> = [];

  @HostBinding('class')
  public workspace = 'pane';

  @ViewChild(ListComponent)
  public list: ListComponent;

  public constructor(
    private _service: WorkflowService
  ) { }

  public ngOnInit(): void {
    this.loadPage(this._page);
  }

  public loadNextPage(): void {
    if (this._page.totalPages - 1 > this._page.pageIndex) {
      this.loadPage(PagingnModel.createNextPage(this._page.pageIndex));
    }
  }

  private loadPage(page: PagingnModel): void {
    this._workflows$ = this._service.workflows(page)
      .subscribe((response: any) => {
        const xPagination = JSON.parse(response.headers.get('X-Pagination'));
        this._page = PagingnModel.fromResponse(xPagination);

        this.list.attachRows(response.body);
      });
  }
}
