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
    <div class="btn-group float-right"
         role="group"
         *ngIf="triggerInfo.triggers">
      <button type="button"
              class="btn"
              *ngFor="let trigger of triggerInfo.triggers"
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

  @Output()
  public trigger: EventEmitter<string> = new EventEmitter<string>();

  public triggerClick(trigger: string): void {
    this.trigger.next(trigger);
  }
}
