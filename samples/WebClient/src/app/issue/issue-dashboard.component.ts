import { Observable } from 'rxjs';

import { Component, OnInit, HostBinding } from '@angular/core';
import { Router } from '@angular/router';

import { AutoUnsubscribe } from './../shared/services/autoUnsubscribe';

import { IssueService } from './issue.service';
import { Issue } from './models';

@AutoUnsubscribe
@Component({
  selector: 'tw-issue-dashboard',
  providers: [IssueService],
  template: `
  <div class="pane__left">
    <div class="btn-group float-right">
      <a class="btn btn-primary" [routerLink]="['detail/new']">
        <tw-icon name="plus"></tw-icon>
      </a>
      <button type="button" class="btn btn-secondary" (click)="reload()">
        <tw-icon name="refresh"></tw-icon>
      </button>
    </div>
    <div class="table-responsive-md">
      <table class="table table-hover">
        <thead>
          <tr>
            <th scope="col" i18n>State</th>
            <th scope="col" i18n>Assignee</th>
            <th scope="col" i18n>Title</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let issue of myWork$ | async">
            <td>{{ issue?.state }}</td>
            <td>{{ issue?.assignee }}</td>
            <td>{{ issue?.title }}</td>
            <td>
              <a [routerLink]="['detail', issue?.id]" i18n>
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
export class IssueDashboardComponent implements OnInit {
  public myWork$: Observable<Array<Issue>>;

  @HostBinding('class')
  public workspace = 'pane';

  public constructor(
    private _router: Router,
    private _service: IssueService
  ) { }

  public ngOnInit(): void {
    this.myWork$ = this._service.myWork();
  }

  public create(): void {
    this._router.navigate(['./detail/new']);
  }

  public reload(): void {
    this.ngOnInit();
  }
}
