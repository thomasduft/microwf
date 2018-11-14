import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IconModule } from './icon/icon.module';
import { ListModule } from './list/list.module';
import { PaneModule } from './pane/pane.module';
import { ScrollerModule } from './scroller/scroller.module';
import { ServicesModule } from './services/services.module';

@NgModule({
  imports: [
    CommonModule,
    IconModule,
    ListModule,
    PaneModule,
    ServicesModule,
    ScrollerModule
  ],
  declarations: [],
  exports: [
    IconModule,
    ListModule,
    PaneModule,
    ScrollerModule
  ]
})
export class SharedModule { }
