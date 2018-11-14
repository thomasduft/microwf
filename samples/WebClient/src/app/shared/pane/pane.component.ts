import {
  Component,
  HostBinding,
  Input
} from '@angular/core';

@Component({
  selector: 'tw-pane',
  template: `
  <ng-content select="[left-pane]" *ngIf="!hideLeftPane">
  </ng-content>
  <ng-content select="[main-pane]">
  </ng-content>
  `
})
export class PaneComponent {
  @HostBinding('class')
  public class = 'grid grid--full';

  @Input()
  public hideLeftPane = false;
}
