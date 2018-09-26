import { NgModule } from '@angular/core';

import { ScrollerDirective } from './scroller.directive';

@NgModule({
  declarations: [
    ScrollerDirective
  ],
  exports: [
    ScrollerDirective
  ]
})
export class ScrollerModule {}
