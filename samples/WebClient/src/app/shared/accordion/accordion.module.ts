import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccordionComponent } from './accordion.component';
import { AccordionGroupComponent } from './accordion-group.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    AccordionComponent,
    AccordionGroupComponent
  ],
  exports: [
    AccordionComponent,
    AccordionGroupComponent
  ]
})
export class AccordionModule { }
