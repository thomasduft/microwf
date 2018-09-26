import { Subscription } from 'rxjs';

import { Component, OnInit, HostBinding } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { AutoUnsubscribe } from '../shared/services/autoUnsubscribe';

import {
  WorkflowService,
  DotInfo,
  Workflow,
  WorkflowHistory,
  WorkflowVariable
} from '../workflow/index';

@AutoUnsubscribe
@Component({
  selector: 'tw-workflow',
  template: `
  <div>
    <div *ngIf="workflow">
      <tw-workflow-list-item [workflow]="workflow">
      </tw-workflow-list-item>
    </div>
  </div>
  <div>
    <div *ngIf="history">
      <table>
        <thead>
          <tr>
            <th i18n>Created</th>
            <th i18n>From</th>
            <th i18n>To</th>
            <th i18n>User</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let h of history">
            <td>{{ h.created | date }}</td>
            <td>{{ h.fromState }}</td>
            <td>{{ h.toState }}</td>
            <td>{{ h.userName }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
  <div>
    <div *ngIf="variables">
      <table>
        <thead>
          <tr>
            <th i18n>Content</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let v of variables">
            <td>{{ v.content }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
  <div>
    <div *ngIf="dot">
      <tw-dot [dot]="dot"></tw-dot>
    </div>
  </div>
  `
})
export class WorkflowComponent implements OnInit {
  private _routeParams$: Subscription;
  private _workflow$: Subscription;
  private _dot$: Subscription;
  private _history$: Subscription;
  private _variables$: Subscription;

  public workflow: Workflow;
  public dot: DotInfo;
  public history: WorkflowHistory[];
  public variables: WorkflowVariable[];

  @HostBinding('class')
  public class = 'grid';

  public constructor(
    private _route: ActivatedRoute,
    private _workflowService: WorkflowService
  ) { }

  public ngOnInit(): void {
    this._routeParams$ = this._route.params
      .subscribe((params: Params) => {
        if (params.id) {
          this.init(params.id);
        }
      });
  }

  private init(id: string): void {
    this._workflow$ = this._workflowService.get(Number(id))
      .subscribe((workflow: Workflow) => {
        this.workflow = workflow;

        // TODO: make use of forkJoin!
        this.loadDot(this.workflow.type);
        this.loadHistory(this.workflow.id);
        this.loadVariables(this.workflow.id);
      });
  }

  private loadDot(key: string): void {
    this._dot$ = this._workflowService.dot(key)
      .subscribe((dot: string) => {
        this.dot = { dot: dot };
      });
  }

  private loadHistory(id: number): void {
    this._history$ = this._workflowService.getHistory(id)
      .subscribe((history: WorkflowHistory[]) => {
        this.history = history;
      });
  }

  private loadVariables(id: number): void {
    this._history$ = this._workflowService.getVariables(id)
      .subscribe((variables: WorkflowVariable[]) => {
        this.variables = variables;
      });
  }
}
