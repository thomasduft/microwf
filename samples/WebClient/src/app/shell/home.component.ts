import { Component, OnInit } from '@angular/core';

import {
  WorkflowArea,
  WorkflowAreaRegistry,
  WorkflowDefinitionService,
  WorkflowDefinition,
  DotInfo
} from '../workflow/index';

@Component({
  selector: 'tw-home',
  template: `
  <h1 i18n>Workflow areas</h1>
  <div class="row">
    <div class="col table-responsive-md">
      <table class="table table-hover" *ngIf="areas.length > 0">
        <thead>
          <tr>
            <th scope="col" i18n>Title</th>
            <th scope="col" i18n>Description</th>
            <th scope="col" i18n>Visualize</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
          <tr *ngFor="let area of areas">
            <td>{{ area.title }}</td>
            <td>{{ area.description }}</td>
            <td>
              <a href="javascript:void(0);" (click)="loadDot(area.key)">
                ...
              </a>
            </td>
            <td><a routerLink="/{{ area.route }}" i18n>let's go</a></td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
  <div class="row" *ngIf="dot">
    <div class="col">
      <tw-dot [dot]="dot"></tw-dot>
    </div>
  </div>
  `
})
export class HomeComponent implements OnInit {
  public dot: DotInfo;

  public get areas(): Array<WorkflowArea> {
    return this._workflowAreaRegistry.areas;
  }

  public constructor(
    private _workflowDefinitionService: WorkflowDefinitionService,
    private _workflowAreaRegistry: WorkflowAreaRegistry
  ) { }

  public ngOnInit(): void {
    this._workflowDefinitionService.definitions()
      .subscribe((definitions: Array<WorkflowDefinition>) => {
        this.init(definitions);
      });
  }

  public loadDot(key: string): void {
    this._workflowDefinitionService.dot(key)
      .subscribe((dot: string) => {
        this.dot = { dot: dot };
      });
  }

  private init(definitions: Array<WorkflowDefinition>): void {
    definitions.forEach((d: WorkflowDefinition) => {
      this._workflowAreaRegistry
        .register(new WorkflowArea(d.type, d.title, d.description, d.route));
    });
  }
}
