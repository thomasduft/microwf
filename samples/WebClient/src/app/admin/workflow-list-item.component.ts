import { Component, Input, HostBinding } from '@angular/core';
import { Workflow } from '../workflow';

@Component({
  selector: 'tw-workflow-list-item',
  template: `
  <p [attr.title]="workflow?.description">
    <a [routerLink]="['detail', workflow?.id]" i18n>
    {{ workflow?.title }}
    </a>
  </p>
  <div class="list__itemDetail">
    <p><b>State:</b>{{ workflow?.state }}</p>
    <p>
      <b>Assignee:</b>{{ workflow?.assignee }}
      <span class="list_item--right"><b>Started:</b>{{ workflow?.started | date }}</span>
    </p>
  </div>`
})
export class WorkflowListItemComponent {
  @HostBinding('class')
  public listItem = 'list__item';

  @Input()
  public workflow: Workflow;
}
