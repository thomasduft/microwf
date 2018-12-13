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
    <tw-list #list [rows]="myWork$ | async">
      <tw-header>
        <h3 i18n>Issues</h3>
        <div class="button__bar">
          <a class="button" [routerLink]="['detail/new']">
            <tw-icon name="plus"></tw-icon>
          </a>
          <button type="button" (click)="reload()">
            <tw-icon name="refresh"></tw-icon>
          </button>
        </div>
      </tw-header>
      <ng-template let-issue twTemplate="issue-item">
        <div class="list__item">
          <div [routerLink]="['detail', issue?.id]" routerLinkActive="active">
            <p>
              <b i18n>Title:</b><span> {{ issue?.title }}</span>
            </p>
            <div class="list__itemDetail">
              <p>
                <b i18n>State:</b><span> {{ issue?.state }}</span>
              </p>
              <p>
                <b i18n>Assignee:</b><span> {{ issue?.assignee }}</span>
              </p>
            </div>
          </div>
        </div>
      </ng-template>
    </tw-list>
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
