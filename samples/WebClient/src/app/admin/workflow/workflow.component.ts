import { Subscription } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { AutoUnsubscribe } from '../../shared/services/autoUnsubscribe';

import {
  WorkflowService,
  DotInfo,
  Workflow,
  WorkflowHistory,
  WorkflowVariable
} from '../../workflow/index';

@AutoUnsubscribe
@Component({
  selector: 'tw-workflow',
  template: `
  <div class="workflow">
    <h3>Overview</h3>
    <div *ngIf="workflow">
      <tw-workflow-list-item [workflow]="workflow">
      </tw-workflow-list-item>
    </div>
    <div *ngIf="dot">
      <tw-dot [dot]="dot"></tw-dot>
    </div>
    <div *ngIf="history && history.length > 0">
      <h3>History</h3>
      <table>
        <thead>
          <tr>
            <th>Created</th>
            <th>From</th>
            <th>To</th>
            <th>User</th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let h of history">
            <td>{{ h.created | date:'MM/dd/yyyy HH:mm:ss' }}</td>
            <td>{{ h.fromState }}</td>
            <td>{{ h.toState }}</td>
            <td>{{ h.userName }}</td>
          </tr>
        </tbody>
      </table>
    </div>
    <div *ngIf="variables && variables.length > 0">
      <h3>Variables</h3>
      <table>
        <thead>
          <tr>
            <th>Content</th>
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
  `
})
export class WorkflowComponent implements OnInit {
  private routeParams$: Subscription;
  private workflow$: Subscription;
  private dot$: Subscription;
  private history$: Subscription;
  private variables$: Subscription;

  public workflow: Workflow;
  public dot: DotInfo;
  public history: WorkflowHistory[];
  public variables: WorkflowVariable[];

  public constructor(
    private _route: ActivatedRoute,
    private _workflowService: WorkflowService
  ) { }

  public ngOnInit(): void {
    this.routeParams$ = this._route.params
      .subscribe((params: Params) => {
        if (params.id) {
          this.init(params.id);
        }
      });
  }

  private init(id: string): void {
    this.workflow$ = this._workflowService.get(Number(id))
      .subscribe((workflow: Workflow) => {
        this.workflow = workflow;

        // TODO: make use of forkJoin!
        this.loadDot(this.workflow.type, this.workflow.correlationId);
        this.loadHistory(this.workflow.id);
        this.loadVariables(this.workflow.id);
      });
  }

  private loadDot(key: string, correlationId: number): void {
    this.dot$ = this._workflowService.dotWithHistory(key, correlationId)
      .subscribe((dot: string) => {
        this.dot = { dot: dot };
      });
  }

  private loadHistory(id: number): void {
    this.history$ = this._workflowService.getHistory(id)
      .subscribe((history: WorkflowHistory[]) => {
        this.history = history;
      });
  }

  private loadVariables(id: number): void {
    this.history$ = this._workflowService.getVariables(id)
      .subscribe((variables: WorkflowVariable[]) => {
        this.variables = variables;
      });
  }
}
