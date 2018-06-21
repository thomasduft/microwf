import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import { DispatchWorkflowComponent } from './dispatch-workflow.component';

const ROUTES: Routes = [
  { path: 'dispatch', component: DispatchWorkflowComponent }
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(ROUTES)
  ],
  declarations: [
    DispatchWorkflowComponent
  ]
})
export class WorkflowModule { }
