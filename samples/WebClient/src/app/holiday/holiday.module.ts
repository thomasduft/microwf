import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes, Router } from '@angular/router';

import { HolidayDashboardComponent } from './holiday-dashboard.component';

const ROUTES: Routes = [
  { path: '', component: HolidayDashboardComponent }
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(ROUTES)
  ],
  declarations: [
    HolidayDashboardComponent
  ],
  exports: [
    RouterModule
  ]
})
export class HolidayModule { }
