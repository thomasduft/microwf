import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { ShellComponent } from './shell.component';


@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild([
      { path: '', component: ShellComponent }
    ])
  ],
  declarations: [ShellComponent]
})
export class ShellModule { }
