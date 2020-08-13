import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SidebarModule } from './sidebar/sidebar.module';
import { WorkspaceModule } from './workspace/workspace.module';

import { ShellComponent } from './shell.component';
import { ForbiddenComponent } from './forbidden.component';
import { PageNotFoundComponent } from './page-not-found.component';

@NgModule({
  imports: [
    CommonModule,
    SidebarModule,
    WorkspaceModule
  ],
  declarations: [
    ShellComponent,
    ForbiddenComponent,
    PageNotFoundComponent
  ],
  exports: [
    ShellComponent
  ]
})
export class ShellModule { }
