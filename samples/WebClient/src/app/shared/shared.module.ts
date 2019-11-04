import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccordionModule } from './accordion/accordion.module';
import { IconModule } from './icon/icon.module';
import { ListModule } from './list/list.module';
import { ScrollerModule } from './scroller/scroller.module';
import { ServicesModule } from './services/services.module';

@NgModule({
  imports: [
    CommonModule,
    AccordionModule,
    IconModule,
    ListModule,
    ServicesModule,
    ScrollerModule
  ],
  declarations: [],
  exports: [
    AccordionModule,
    IconModule,
    ListModule,
    ScrollerModule
  ]
})
export class SharedModule { }
