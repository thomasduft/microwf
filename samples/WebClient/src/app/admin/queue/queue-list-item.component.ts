import { Component, Input, HostBinding } from '@angular/core';
import { QueueItem } from '../../workflow';

@Component({
  selector: 'tw-queue-list-item',
  template: `
  <div [routerLink]="['detail', queueItem?.id]" routerLinkActive="active">
    <p [attr.title]="queueItem?.workflowType">
      <b>{{ queueItem?.triggerName }} #{{queueItem?.entityId}}</b>
    </p>
    <div class="list__itemDetail">
      <p>
        <b>Error:</b> {{ queueItem?.error }}
      </p>
      <p>
        <b>Retries:</b> {{ queueItem?.retries }}
        <span class="list_item--right">
          <b>DueDate:</b> {{ queueItem?.dueDate | date : 'MM/dd/yyyy - hh:mm:ss' }}
        </span>
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
