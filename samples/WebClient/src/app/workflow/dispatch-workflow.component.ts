import { Subscription } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AutoUnsubscribe } from '../shared/services/autoUnsubscribe';

@AutoUnsubscribe
@Component({
  selector: 'tw-dispatch-workflow',
  template: `
  <div class="pane__main pane--space">
    <p i18n>This workflow has beeing dispatched to <b>{{ assignee }}</b>!</p>
    <button type="button"
            (click)="goTo()"
            i18n>back</button>
  </div>`
})
export class DispatchWorkflowComponent implements OnInit {
  private _routeParams$: Subscription;

  public assignee: string;
  public goto: string;

  public constructor(
    private _route: ActivatedRoute,
    private _router: Router
  ) { }

  public ngOnInit(): void {
    this._routeParams$ = this._route.params
      .subscribe((params: Params) => {
        this.assignee = params.assignee;
        this.goto = params.goto;
      });
  }

  public goTo(): void {
    this._router.navigate([this.goto]);
  }
}
