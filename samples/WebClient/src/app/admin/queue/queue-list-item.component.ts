import { Component, Input, HostBinding } from '@angular/core';
import { QueueItem } from '../../workflow';

@Component({
  selector: 'tw-queue-list-item',
  template: `
  <div
    [routerLink]="['.', queueItem?.id]"
    routerLinkActive="active"
    [attr.title]="queueItem?.workflowType">
    <p>
      <b>{{ queueItem?.triggerName }} #{{queueItem?.entityId}}</b>
    </p>
    <div class="list__itemDetail">
      <p>
        <b>Error:</b> {{ queueItem?.error }}
      </p>
      <p>
        <b>Retries:</b> {{ queueItem?.retries }}
      </p>
      <p>
        <b>DueDate:</b> {{ queueItem?.dueDate | date : 'MM/dd/yyyy-hh:mm:ss' }}
      </p>
    </div>
  </div>`
})
export class QueueListItemComponent {
  @HostBinding('class')
  public listItem = 'list__item';

  @Input()
  public queueItem: QueueItem;
}
