import { Component } from '@angular/core';

import {
  WorkflowArea,
  WorkflowAreaRegistry,
  WorkflowService,
  DotInfo
} from '../workflow/index';

@Component({
  selector: 'tw-home',
  template: `
  <div class="pane__main">
    <h1 i18n>Workflows</h1>
    <div>
      <table *ngIf="areas.length > 0">
        <thead>
          <tr>
            <th scope="col" i18n>Title</th>
            <th scope="col" i18n>Description</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let area of areas" (mouseover)="loadDot(area.key)">
            <td>{{ area.title }}</td>
            <td>{{ area.description }}</td>
            <td>
              <a routerLink="/{{ area.route }}" i18n>
                <tw-icon name="arrow-right"></tw-icon>
              </a>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div *ngIf="dot">
      <div class="col">
        <tw-dot [dot]="dot"></tw-dot>
      </div>
    </div>
  </div>
  `
})
export class HomeComponent {
  public dot: DotInfo;

  public get areas(): Array<WorkflowArea> {
    return this._workflowAreaRegistry.areas;
  }

  public constructor(
    private _workflowService: WorkflowService,
    private _workflowAreaRegistry: WorkflowAreaRegistry
  ) { }

  public loadDot(key: string): void {
    this._workflowService.dot(key)
      .subscribe((dot: string) => {
        this.dot = { dot: dot };
      });
  }
}
