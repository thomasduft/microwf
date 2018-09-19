import { Observable } from 'rxjs';

import { Component, OnInit, HostBinding, ViewChild } from '@angular/core';

import { AutoUnsubscribe } from './../shared/services/autoUnsubscribe';
import { ListComponent } from '../shared/list/list.component';

import { WorkflowService, Workflow } from './../workflow/index';

@AutoUnsubscribe
@Component({
  selector: 'tw-admin-dashboard',
  providers: [WorkflowService],
  template: `
  <div class="pane__left">
    <tw-list #list [rows]="workflows$ | async">
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
  public workflows$: Observable<Array<Workflow>>;

  @HostBinding('class')
  public workspace = 'pane';

  @ViewChild(ListComponent)
  public list: ListComponent;

  public constructor(
    private _service: WorkflowService
  ) { }

  public ngOnInit(): void {
    this.workflows$ = this._service.workflows();
  }

  public reload(): void {
    this.ngOnInit();
  }
}
