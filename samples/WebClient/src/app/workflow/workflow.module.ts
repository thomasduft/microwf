import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DispatchWorkflowComponent } from './dispatch-workflow.component';
import { TriggerInfoComponent } from './trigger-info.component';

import { VizModule } from './viz/viz.module';

@NgModule({
  imports: [
    CommonModule,
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
