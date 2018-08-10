import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IconModule } from './icon/icon.module';
import { ServicesModule } from './services/services.module';

@NgModule({
  imports: [
    CommonModule,
    IconModule,
    ServicesModule
  ],
  declarations: [],
  exports: [
    IconModule
  ]
})
export class SharedModule { }
