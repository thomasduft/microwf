import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IconModule } from './icon/icon.module';
import { ListModule } from './list/list.module';
import { ScrollerModule } from './scroller/scroller.module';
import { ServicesModule } from './services/services.module';
import { TabModule } from './tab/tab.module';

@NgModule({
  imports: [
    CommonModule,
    IconModule,
    ListModule,
    ServicesModule,
    ScrollerModule,
    TabModule
  ],
  declarations: [],
  exports: [
    IconModule,
    ListModule,
    ScrollerModule,
    TabModule
  ]
})
export class SharedModule { }
