import {
  Component,
  HostBinding
} from '@angular/core';

import { AuthService } from '../../shared/services/auth.service';

@Component({
  selector: 'tw-sidebar',
  template: `
  <div class="sidebar__header">
    <a routerLink="/home" i18n></a>
  </div>
  <div class="sidebar__content">
    <ul>
      <li routerLinkActive="active">
        <a routerLink="/home">
          <tw-icon name="home"></tw-icon>
          Home
        </a>
      </li>
      <tw-menu></tw-menu>
      <li routerLinkActive="active" *ngIf="isAdmin">
        <span>
          <tw-icon name="wrench"></tw-icon>
          Admin
          <ul>
            <li routerLinkActive="active">
              <a routerLink="/workflows">
                <tw-icon name="feather"></tw-icon>
                Workflows
              </a>
            </li>
          </ul>
        </span>
      </li>
      <li routerLinkActive="active">
        <a (click)="logout()">
          <tw-icon name="sign-out"></tw-icon>
          Log out
        </a>
      </li>
    </ul>
  </div>
  <div class="sidebar__footer"></div>
  `
})
export class SidebarComponent {
  @HostBinding('class')
  public sidebar = 'sidebar';

  public get isAdmin(): boolean {
    return this._authService.hasClaim('workflow_admin');
  }

  public constructor(
    private _authService: AuthService
  ) { }

  public logout(): void {
    this._authService.logout();
  }
}
