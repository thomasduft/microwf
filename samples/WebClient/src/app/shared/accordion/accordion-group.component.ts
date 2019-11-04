import {
  Component,
  Input,
  Output,
  EventEmitter,
  HostBinding
} from '@angular/core';

@Component({
  selector: 'tw-accordion-group',
  template: `
  <div class="accordion__group-title" (click)="toggle.emit(title)">
    {{title}}
  </div>
  <div class="accordion__group-body" [ngClass]="{'accordion__group-body--hidden': !opened}">
    <ng-content></ng-content>
  </div>
  `
})
export class AccordionGroupComponent {
  @HostBinding('class')
  public style = 'accordion__group';

  @Input()
  public opened = false;

  @Input()
  public title: string;

  @Output()
  public toggle: EventEmitter<string> = new EventEmitter<string>();
}
