import { Observable } from 'rxjs';

import { Component, OnInit } from '@angular/core';

import { AutoUnsubscribe } from './../shared/services/autoUnsubscribe';

import { WorkflowService } from './workflow.service';
import { Workflow } from './models';

@AutoUnsubscribe
@Component({
  selector: 'tw-workflow-dashboard',
  providers: [WorkflowService],
  template: `
  <div class="pane__left">
    <div class="table-responsive-md">
      <table class="table table-hover">
        <thead>
          <tr>
            <th scope="col" i18n>Title</th>
            <th scope="col" i18n>State</th>
            <th scope="col" i18n>Assignee</th>
            <th scope="col" i18n>Started</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let workflow of workflows$ | async">
            <td [attr.title]="workflow?.description">{{ workflow?.title }}</td>
            <td>{{ workflow?.state }}</td>
            <td>{{ workflow?.assignee }}</td>
            <td>{{ workflow?.started | date }}</td>
            <td>
              <a [routerLink]="['/' + workflow?.route + '/detail/', workflow?.id]" i18n>
                <tw-icon name="arrow-right"></tw-icon>
              </a>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
  <div class="pane__main">
    <router-outlet></router-outlet>
  </div>`
})
export class WorkflowDashboardComponent implements OnInit {
  public workflows$: Observable<Array<Workflow>>;

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
