import {
  Component,
  HostBinding
} from '@angular/core';

import { OAuthService } from 'angular-oauth2-oidc';

import { UserService } from '../../shared/services/user.service';

@Component({
  selector: 'tw-sidebar',
  template: `
  <div class="sidebar__header" (click)="toggle()">
    <div *ngIf="!collapsed">MicroWF</div>
    <button type="button" >
      <tw-icon *ngIf="collapsed" name="arrow-right"></tw-icon>
      <tw-icon *ngIf="!collapsed" name="arrow-left"></tw-icon>
    </button>
  </div>
  <div class="sidebar__content">
    <ul>
      <li routerLinkActive="active" class="menu__item">
        <a routerLink="/home">
          <tw-icon name="home"></tw-icon>
          {{ userName }}
        </a>
      </li>
      <tw-menu></tw-menu>
      <li *ngIf="isAdmin">
        <span>
          <tw-icon name="wrench"></tw-icon>
          Admin
          <ul>
            <li routerLinkActive="active" class="menu__item">
              <a routerLink="/admin/workflows">
                <tw-icon name="arrow-right"></tw-icon>
                Workflows
              </a>
            </li>
            <li routerLinkActive="active" class="menu__item">
              <a routerLink="/admin/queue">
                <tw-icon name="cog"></tw-icon>
                Jobqueue
              </a>
            </li>
          </ul>
        </span>
      </li>
    </ul>
  </div>
  <div class="sidebar__footer">
    <ul>
      <li routerLinkActive="active">
        <a href="javascript:void(0)" (click)="logout()">
          <tw-icon name="sign-out"></tw-icon>
          Log out
        </a>
      </li>
    </ul>
  </div>
  `
})
export class SidebarComponent {
  public collapsed = false;

  @HostBinding('class')
  public classlist = this.getClassList();

  public get userName(): string {
    return this.authService.userName;
  }

  public get isAdmin(): boolean {
    return this.authService.hasClaim('workflow_admin');
  }

  public constructor(
    private oauthService: OAuthService,
    private authService: UserService
  ) { }

  public logout(): void {
    this.oauthService.logOut();
  }

  public toggle(): void {
    this.collapsed = !this.collapsed;
    this.classlist = this.getClassList();
  }

  private getClassList(): string {
    if (this.collapsed) {
      return 'sidebar sidebar--collapsed';
    }

    return 'sidebar';
  }
}
