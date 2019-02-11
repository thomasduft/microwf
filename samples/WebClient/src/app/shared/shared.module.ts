import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IconModule } from './icon/icon.module';
import { ListModule } from './list/list.module';
import { ScrollerModule } from './scroller/scroller.module';
import { ServicesModule } from './services/services.module';

@NgModule({
  imports: [
    CommonModule,
    IconModule,
    ListModule,
    ServicesModule,
    ScrollerModule
  ],
  declarations: [],
  exports: [
    IconModule,
    ListModule,
    ScrollerModule
  ]
})
export class SharedModule { }
