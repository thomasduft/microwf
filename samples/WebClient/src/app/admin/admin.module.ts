import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { IconModule } from './../shared/icon/icon.module';
import { FormdefModule } from './../shared/formdef/index';
import { WorkflowModule } from '../workflow/workflow.module';

import { AdminDashboardComponent } from './admin-dashboard.component';
import { AdministratorClaimGuard } from './administratorClaimGuard';

const ROUTES: Routes = [
  { path: '', component: AdminDashboardComponent, canActivate: [AdministratorClaimGuard] }
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(ROUTES),
    FormdefModule,
    WorkflowModule,
    IconModule
  ],
  declarations: [
    AdminDashboardComponent
  ],
  providers: [
    AdministratorClaimGuard
  ]
})
export class AdminModule { }
