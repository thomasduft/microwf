import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { IconModule } from '../shared/icon/icon.module';
import { ListModule } from './../shared/list/list.module';
import { FormdefModule } from '../shared/formdef/index';
import { WorkflowModule } from '../workflow/workflow.module';

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
    IconModule,
    ListModule
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
}

