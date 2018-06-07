import { Component } from '@angular/core';

import { AuthService } from '../shared/services/auth.service';

@Component({
  selector: 'tw-shell',
  template: `
  <nav class="navbar navbar-expand-md navbar-dark bg-dark fixed-top">
    <a class="navbar-brand" routerLink="/home">MircoWF - {{ username }}</a>

    <div class="navbar-collapse">
      <ul class="navbar-nav ml-auto">
        <li *ngIf="isAuthenticated">
          <a class="nav-link" href="javascript:void(0)" (click)="logout()" i18n>Logout</a>
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
  </nav>

  <main role="main" class="container">
    <router-outlet></router-outlet>
  </main>`
})
export class ShellComponent {
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
