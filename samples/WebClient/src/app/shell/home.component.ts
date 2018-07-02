import { Component, OnInit } from '@angular/core';

import {
  WorkflowArea,
  WorkflowAreaRegistry,
  WorkflowDefinitionService,
  WorkflowDefinition
} from '../workflow/index';

@Component({
  selector: 'tw-home',
  template: `
  <h1 i18n>Workflow areas</h1>
  <div class="table-responsive-md">
    <table class="table table-hover" *ngIf="areas.length > 0">
      <thead>
        <tr>
          <th scope="col" i18n>Title</th>
          <th scope="col" i18n>Description</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let area of areas">
          <td>{{ area.title }}</td>
          <td>{{ area.description }}</td>
          <td><a routerLink="/{{ area.route }}" i18n>let's go</a></td>
        </tr>
      </tbody>
    </table>
  </div>`
})
export class HomeComponent implements OnInit {
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

  private init(definitions: Array<WorkflowDefinition>): void {
    definitions.forEach((d: WorkflowDefinition) => {
      this._workflowAreaRegistry
        .register(new WorkflowArea(d.type, d.title, d.description, d.route));
    });
  }
}
