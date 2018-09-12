import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { IconModule } from '../shared/icon/icon.module';
import { PaneModule } from '../shared/pane/pane.module';
import { FormdefModule, FormdefRegistry } from '../shared/formdef/index';
import { WorkflowModule } from '../workflow/workflow.module';

import { IssueDetailSlot } from './models';
import { IssueService } from './issue.service';
import { IssueDashboardComponent } from './issue-dashboard.component';
import { IssueComponent } from './issue.component';

const ROUTES: Routes = [
  {
    path: '', component: IssueDashboardComponent, children: [
      { path: 'detail/:id', component: IssueComponent },
      { path: 'detail/new', component: IssueComponent }
    ]
  },
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(ROUTES),
    FormdefModule,
    WorkflowModule,
    PaneModule,
    IconModule
  ],
  declarations: [
    IssueDashboardComponent,
    IssueComponent
  ],
  providers: [
    IssueService
  ],
  exports: [
    RouterModule
  ]
})
export class IssueModule {
  public constructor(
    private _slotRegistry: FormdefRegistry
  ) {
    this._slotRegistry.register(new IssueDetailSlot());
  }
}

