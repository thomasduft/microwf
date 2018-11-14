import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { IconModule } from './../shared/icon/icon.module';
import { ListModule } from './../shared/list/list.module';
import { FormdefModule, FormdefRegistry } from './../shared/formdef/index';
import { WorkflowModule } from '../workflow/workflow.module';

import { HolidayComponent } from './holiday.component';
import { HolidayDashboardComponent } from './holiday-dashboard.component';
import { HolidayService } from './holiday.service';
import { ApplyHolidayDetailSlot, ApproveHolidayDetailSlot } from './models';

const ROUTES: Routes = [
  {
    path: '', component: HolidayDashboardComponent, children: [
      { path: 'detail/:id', component: HolidayComponent },
      { path: 'detail/new', component: HolidayComponent }
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
export class HolidayModule {
  public constructor(
    private _slotRegistry: FormdefRegistry
  ) {
    this._slotRegistry.register(new ApplyHolidayDetailSlot());
    this._slotRegistry.register(new ApproveHolidayDetailSlot());
  }
}
