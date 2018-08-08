import { Observable } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AutoUnsubscribe } from './../shared/services/autoUnsubscribe';

import { IssueService } from './issue.service';
import { Issue } from './models';

@AutoUnsubscribe
@Component({
  selector: 'tw-issue-dashboard',
  providers: [IssueService],
  template: `
  <h1 i18n>Issue</h1>
  <hr />
  <div class="btn-group float-right">
    <a class="btn btn-primary" [routerLink]="['detail/new']" i18n>New</a>
    <button type="button" class="btn btn-secondary" (click)="reload()">reload</button>
  </div>
  <h2 i18n>My work</h2>
  <div class="table-responsive-md">
    <table class="table table-hover">
      <thead>
        <tr>
          <th scope="col" i18n>State</th>
          <th scope="col" i18n>Assignee</th>
          <th scope="col" i18n>Title</th>
          <th scope="col" i18n>Action</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let issue of myWork$ | async">
          <td>{{ issue?.state }}</td>
          <td>{{ issue?.assignee }}</td>
          <td>{{ issue?.title }}</td>
          <td>
            <a [routerLink]="['detail', issue?.id]" i18n>open</a>
          </td>
        </tr>
      </tbody>
    </table>
  </div>`
})
export class IssueDashboardComponent implements OnInit {
  public myWork$: Observable<Array<Issue>>;

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
