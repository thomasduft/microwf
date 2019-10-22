import { Component, Input, HostBinding } from '@angular/core';
import { Workflow } from '../../workflow';

@Component({
  selector: 'tw-workflow-list-item',
  template: `
  <div [routerLink]="['detail', workflow?.id]" routerLinkActive="active">
    <p [attr.title]="workflow?.description">
      <b>{{ workflow?.title }} #{{workflow?.correlationId}}</b>
    </p>
    <div class="list__itemDetail">
      <p>
        <b>State:</b> {{ workflow?.state }}
        <span class="list_item--right">
          <b>Assignee:</b> {{ workflow?.assignee }}
        </span>
      </p>
      <p>
        <b>Start:</b> {{ workflow?.started | date }}
        <span *ngIf="workflow?.completed" class="list_item--right">
          <b>End:</b> {{ workflow?.completed | date }}
        </span>
      </p>
    </div>
  </div>`
})
export class WorkflowListItemComponent {
  @HostBinding('class')
  public listItem = 'list__item';

  @Input()
  public workflow: Workflow;
}
