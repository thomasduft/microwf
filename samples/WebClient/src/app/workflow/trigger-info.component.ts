import {
  Component,
  Input,
  Output,
  EventEmitter
} from '@angular/core';

import { TriggerInfo } from './models';

@Component({
  selector: 'tw-trigger-info',
  template: `
  <div *ngIf="triggerInfo">
    <div class="button__bar"
         *ngIf="triggerInfo.triggers">
      <button type="button"
              class="button--secondary"
              *ngFor="let trigger of triggerInfo.triggers"
              [disabled]="!canTrigger"
              (click)="triggerClick(trigger)">{{ trigger }}
      </button>
    </div>
    <div class="alert alert-danger"
         role="alert"
         *ngIf="triggerInfo.errors">
      <ul>
        <li *ngFor="let error of triggerInfo.errors">{{ error }}</li>
      </ul>
    </div>
  </div>`
})
export class TriggerInfoComponent {
  @Input()
  public triggerInfo: TriggerInfo;

  @Input()
  public canTrigger = false;

  @Output()
  public trigger: EventEmitter<string> = new EventEmitter<string>();

  public triggerClick(trigger: string): void {
    this.trigger.next(trigger);
  }
}
