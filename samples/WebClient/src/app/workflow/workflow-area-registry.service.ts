import { Injectable } from '@angular/core';

import { WorkflowArea } from './models';
import { WorkflowModule } from './workflow.module';

@Injectable({
  providedIn: WorkflowModule
})
export class WorkflowAreaRegistry {
  private _registry: Map<string, WorkflowArea> = new Map<string, WorkflowArea>();

  public get areas(): Array<WorkflowArea> {
    return Array.from(this._registry.values());
  }

  public register(area: WorkflowArea): void {
    if (this._registry.has(area.key)) { return; }

    this._registry.set(area.key, area);
  }

  public exists(key: string): boolean {
    return this._registry.has(key);
  }

  public get(key: string): WorkflowArea {
    return this._registry.get(key);
  }

  public clear(): void {
    this._registry.clear();
  }
}
