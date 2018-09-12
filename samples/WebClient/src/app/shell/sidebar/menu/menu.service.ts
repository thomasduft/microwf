import { Injectable } from '@angular/core';

import { MenuItem } from './models';

@Injectable()
export class MenuService {
  private _items: Array<MenuItem> = new Array<MenuItem>();

  public get items(): Array<MenuItem> {
    return this._items;
  }

  public register(item: MenuItem): void {
    if (!this._items.some((m: MenuItem) => {
      return m.id === item.id;
    })) {
      this._items.push(item);
    }
  }
}
