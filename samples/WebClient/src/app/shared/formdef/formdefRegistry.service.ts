import { Injectable } from '@angular/core';

import { Slot } from './models';

@Injectable()
export class FormdefRegistry {
  private _registry: Map<string, Slot>;

  public constructor() {
    this._registry = new Map<string, Slot>();
  }

  public register(slot: Slot) {
    this._registry.set(slot.key, slot);
  }

  public get(key: string): Slot {
    return this._registry.get(key);
  }
}
