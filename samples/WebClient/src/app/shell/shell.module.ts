import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SidebarModule } from './sidebar/sidebar.module';
import { WorkspaceModule } from './workspace/workspace.module';

import { ShellComponent } from './shell.component';

@NgModule({
  imports: [
    CommonModule,
    SidebarModule,
    WorkspaceModule
  ],
  declarations: [
    ShellComponent
  ]
})
export class ShellModule { }
