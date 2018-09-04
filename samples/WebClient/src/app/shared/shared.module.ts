import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IconModule } from './icon/icon.module';
import { ScrollerModule } from './scroller/scroller.module';
import { ServicesModule } from './services/services.module';

@NgModule({
  imports: [
    CommonModule,
    IconModule,
    ServicesModule,
    ScrollerModule
  ],
  declarations: [],
  exports: [
    IconModule,
    ScrollerModule
  ]
})
export class SharedModule { }
