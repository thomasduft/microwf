import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { IconModule } from './../../shared/icon/icon.module';

import { MenuModule } from './menu/menu.module';

import { SidebarComponent } from './sidebar.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    IconModule,
    MenuModule
  ],
  declarations: [
    SidebarComponent
  ],
  exports: [
    SidebarComponent
  ]
})
export class SidebarModule { }
