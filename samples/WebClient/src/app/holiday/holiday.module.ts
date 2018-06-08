import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { FormdefModule } from './../shared/formdef/index';

import { HolidayDashboardComponent } from './holiday-dashboard.component';

const ROUTES: Routes = [
  { path: '', component: HolidayDashboardComponent }
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(ROUTES),
    FormdefModule
  ],
  declarations: [
    HolidayDashboardComponent
  ],
  exports: [
    RouterModule
  ]
})
export class HolidayModule { }
