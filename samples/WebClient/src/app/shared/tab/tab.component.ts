import { Component, Input, HostBinding } from '@angular/core';

@Component({
  selector: 'tw-tab',
  template: `
  <div *ngIf="active">
    <ng-content></ng-content>
  </div>`
})
export class TabComponent {
  @HostBinding('class')
  public style = 'tabs__tab';

  @Input()
  public title: string;

  @Input()
  public active = false;
}
