import { Component, OnInit } from '@angular/core';

import { MenuItem } from './models';
import { MenuService } from './menu.service';

@Component({
  selector: 'tw-menu',
  providers: [
    MenuService
  ],
  template: `
  <li routerLinkActive="item.route ? 'active' : null"
      [ngClass]="{'menu__item': item.route}"
      *ngFor="let item of menuItems">
    <a *ngIf="item.route" [routerLink]="item.route">
      <tw-icon *ngIf="item.icon" [name]="item.icon"></tw-icon>
      {{ item.name }}
    </a>
    <span *ngIf="!item.route">
      <tw-icon *ngIf="item.icon" [name]="item.icon"></tw-icon>
      {{ item.name }}
      <ul *ngIf="item.children && item.children.length > 0">
        <li routerLinkActive="active" class="menu__item" *ngFor="let child of item.children">
          <a *ngIf="child.route" [routerLink]="child.route">
            <tw-icon *ngIf="child.icon" [name]="child.icon"></tw-icon>
            {{ child.name }}
          </a>
        </li>
      </ul>
    </span>
  </li>
  `
})
export class MenuComponent implements OnInit {
  public get menuItems(): Array<MenuItem> {
    return this._menuService.items;
  }

  public constructor(
    private _menuService: MenuService
  ) { }

  public ngOnInit(): void {
    // this._menuService.register({
    //   id: '2',
    //   name: 'Costs',
    //   route: '#',
    //   icon: 'credit-card'
    // });

    // this._menuService.register({
    //   id: '3',
    //   name: 'Hardworker',
    //   route: '#',
    //   icon: 'building-o'
    // });

    // this._menuService.register({
    //   id: '4',
    //   name: 'Admin',
    //   icon: 'wrench',
    //   children: [
    //     {
    //       id: '4.1',
    //       name: 'Users',
    //       route: '/admin/users',
    //       icon: 'users'
    //     },
    //     {
    //       id: '4.2',
    //       name: 'Register user',
    //       route: '/admin/registeruser',
    //       icon: 'user'
    //     }
    //   ]
    // });
  }
}
