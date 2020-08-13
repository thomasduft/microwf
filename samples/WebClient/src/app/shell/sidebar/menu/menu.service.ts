import { Injectable, OnDestroy } from '@angular/core';

import { MenuItem } from './models';
import {
  WorkflowDefinition,
  WorkflowArea,
  WorkflowService,
  WorkflowAreaRegistry
} from '../../../workflow';
import { UserService } from 'src/app/shared/services/user.service';

@Injectable()
export class MenuService implements OnDestroy {
  private _items: Array<MenuItem> = new Array<MenuItem>();

  public get items(): Array<MenuItem> {
    return this._items;
  }

  public constructor(
    private user: UserService,
    private workflowService: WorkflowService,
    private workflowAreaRegistry: WorkflowAreaRegistry
  ) {
    this.user.authenticated$.subscribe(() => {
      this.workflowService.definitions()
        .subscribe((definitions: Array<WorkflowDefinition>) => {
          this.init(definitions);
          this.createMenu();
        });
    });
  }

  public ngOnDestroy(): void {
    this._items = [];
    this.workflowAreaRegistry.clear();
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
        this.workflowAreaRegistry
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

    this.workflowAreaRegistry.areas.forEach((area: WorkflowArea) => {
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
