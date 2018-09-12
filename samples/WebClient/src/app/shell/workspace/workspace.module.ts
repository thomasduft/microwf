import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { StatusModule } from './status/index';
import { WorkspaceComponent } from './workspace.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    StatusModule
  ],
  declarations: [
    WorkspaceComponent
  ],
  exports: [
    StatusModule,
    WorkspaceComponent
  ]
})
export class WorkspaceModule { }
