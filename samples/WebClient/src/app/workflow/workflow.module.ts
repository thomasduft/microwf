import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DispatchWorkflowComponent } from './dispatch-workflow.component';
import { TriggerInfoComponent } from './trigger-info.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    DispatchWorkflowComponent,
    TriggerInfoComponent
  ],
  exports: [
    TriggerInfoComponent
  ]
})
export class WorkflowModule { }
