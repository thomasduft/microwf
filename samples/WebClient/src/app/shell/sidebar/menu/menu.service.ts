import { Injectable, OnDestroy } from '@angular/core';

import { MenuItem } from './models';
import {
  WorkflowDefinition,
  WorkflowArea,
  WorkflowService,
  WorkflowAreaRegistry
} from '../../../workflow';
import { isNgTemplate } from '@angular/compiler';

@Injectable()
export class MenuService implements OnDestroy {
  private _items: Array<MenuItem> = new Array<MenuItem>();

  public get items(): Array<MenuItem> {
    return this._items;
  }

  public constructor(
    private _workflowService: WorkflowService,
    private _workflowAreaRegistry: WorkflowAreaRegistry
  ) {
    this._workflowService.definitions()
      .subscribe((definitions: Array<WorkflowDefinition>) => {
        this.init(definitions);
        this.createMenu();
      });
  }

  public ngOnDestroy(): void {
    this._items = [];
    this._workflowAreaRegistry.clear();
  }

  public register(item: MenuItem): void {
    if (!this._items.some((m: MenuItem) => {
      return m.id === item.id;
    })) {
      this._items.push(item);
    }
  }

  private init(definitions: Array<WorkflowDefinition>): void {
    definitions.forEach((d: WorkflowDefinition) => {
      if (d.route) {
        this._workflowAreaRegistry
          .register(new WorkflowArea(d.type, d.title, d.description, d.route));
      }
    });
  }

  private createMenu(): void {
    const tasks: MenuItem = {
      id: '1',
      name: 'Tasks',
      icon: 'tasks',
      children: []
    };

    this._workflowAreaRegistry.areas.forEach((area: WorkflowArea) => {
      const child: MenuItem = {
        id: area.key,
        name: area.title,
        route: `/${area.route}`,
        icon: this.getIcon(area.key)
      };
      tasks.children.push(child);
    });

    this.register(tasks);
  }

  private getIcon(key: string): any {
    if (key === 'HolidayApprovalWorkflow') { return 'plane'; }
    if (key === 'IssueTrackingWorkflow') { return 'bug'; }

    return 'tasks';
  }
}
