import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { IconModule } from './../shared/icon/icon.module';
import { ListModule } from './../shared/list/list.module';
import { FormdefModule } from './../shared/formdef/index';
import { WorkflowModule } from '../workflow/workflow.module';

import { AdminDashboardComponent } from './admin-dashboard.component';
import { WorkflowComponent } from './workflow.component';
import { WorkflowListItemComponent } from './workflow-list-item.component';
import { AdministratorClaimGuard } from './administratorClaimGuard';

const ROUTES: Routes = [
  {
    path: '',
    component: AdminDashboardComponent,
    canActivate: [AdministratorClaimGuard],
    children: [
      { path: 'detail/:id', component: WorkflowComponent },
    ]
  }
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
    AdminDashboardComponent,
    WorkflowComponent,
    WorkflowListItemComponent
  ],
  providers: [
    AdministratorClaimGuard
  ]
})
export class AdminModule { }
