import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { FormdefModule } from './../shared/formdef/index';

import { HolidayComponent } from './holiday.component';
import { HolidayDashboardComponent } from './holiday-dashboard.component';

import { HolidayService } from './holiday.service';

const ROUTES: Routes = [
  { path: '', component: HolidayDashboardComponent },
  { path: 'detail/:id', component: HolidayComponent },
  { path: 'detail/new', component: HolidayComponent }
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(ROUTES),
    FormdefModule
  ],
  declarations: [
    HolidayDashboardComponent,
    HolidayComponent
  ],
  providers: [
    HolidayService
  ],
  exports: [
    RouterModule
  ]
})
export class HolidayModule { }
