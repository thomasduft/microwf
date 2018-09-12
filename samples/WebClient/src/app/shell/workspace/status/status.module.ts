import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StatusBarComponent } from './statusbar.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    StatusBarComponent
  ],
  exports: [
    StatusBarComponent
  ]
})
export class StatusModule { }
