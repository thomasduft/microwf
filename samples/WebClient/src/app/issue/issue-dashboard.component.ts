import { Subscription } from 'rxjs';

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
  <h2 i18n>My work</h2>
  <a class="btn btn-primary" [routerLink]="['detail/new']" i18n>New</a>
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
        <tr *ngFor="let issue of myWork">
          <td>{{ issue.state }}</td>
          <td>{{ issue.assignee }}</td>
          <td>{{ issue.title }}</td>
          <td>
            <a [routerLink]="['detail', issue.id]" i18n>open</a>
          </td>
        </tr>
      </tbody>
    </table>
  </div>`
})
export class IssueDashboardComponent implements OnInit {
  private _myWork$: Subscription;

  public myWork: Array<Issue> = [];

  public constructor(
    private _router: Router,
    private _service: IssueService
  ) { }

  public ngOnInit(): void {
    this._myWork$ = this._service.myWork()
      .subscribe((myWork: Array<Issue>) => {
        this.myWork = myWork;
      });
  }

  public create(): void {
    this._router.navigate(['./detail/new']);
  }
}
