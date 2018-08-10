import { Component } from '@angular/core';

import { AuthService } from '../shared/services/auth.service';

@Component({
  selector: 'tw-header',
  template: `
  <nav class="navbar navbar-dark bg-dark fixed-top">
    <a class="navbar-brand" routerLink="/home">MircoWF - {{ username }}</a>
    <div class="navbar">
      <ul class="nav navbar-nav mr-auto">
        <li *ngIf="isAuthenticated">
          <a class="nav-link" href="javascript:void(0)" (click)="logout()">
            <tw-icon name="sign-out"></tw-icon>
          </a>
        </li>
      </ul>
      <form class="form-inline" *ngIf="false">
        <input class="form-control mr-sm-2"
               type="text"
               placeholder="Start a workflow"
               i18n-placeholder
               aria-label="Search">
        <button class="btn btn-secondary" type="button" i18n>Start</button>
      </form>
    </div>
  </nav>`
})
export class HeaderComponent {
  public get username(): string {
    return this._authService.username;
  }

  public get isAuthenticated(): boolean {
    return this._authService.isAuthenticated;
  }

  public constructor(
    private _authService: AuthService
  ) { }

  public logout(): void {
    this._authService.logout();
  }
}
