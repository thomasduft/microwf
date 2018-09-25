import { Component, Input, HostBinding } from '@angular/core';
import { Workflow } from '../workflow';

@Component({
  selector: 'tw-workflow-list-item',
  template: `
  <div [routerLink]="['detail', workflow?.id]" routerLinkActive="active">
    <p [attr.title]="workflow?.description">
      <b>{{ workflow?.title }} #{{workflow?.id}}</b>
    </p>
    <div class="list__itemDetail">
      <p><b>State:</b> {{ workflow?.state }}</p>
      <p>
        <b>Assignee:</b> {{ workflow?.assignee }}
        <span class="list_item--right"><b>Started:</b> {{ workflow?.started | date }}</span>
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
