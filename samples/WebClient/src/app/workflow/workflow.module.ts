import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { IconModule } from './../shared/icon/icon.module';

import { DispatchWorkflowComponent } from './dispatch-workflow.component';
import { TriggerInfoComponent } from './trigger-info.component';

import { VizModule } from './viz/viz.module';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    IconModule,
    VizModule
  ],
  declarations: [
    DispatchWorkflowComponent,
    TriggerInfoComponent
  ],
  exports: [
    VizModule,
    TriggerInfoComponent
  ]
})
export class WorkflowModule { }
