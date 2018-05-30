import { Component, OnInit } from '@angular/core';

import { AuthService } from '../shared/services/auth.service';

@Component({
  selector: 'tw-shell',
  template: `
  <nav class="navbar navbar-expand-md navbar-dark bg-dark fixed-top">
    <a class="navbar-brand" routerLink="welcome">MircoWF - {{ username }}</a>
    <button class="navbar-toggler" type="button" data-toggle="collapse"
      data-target="#navbarsExampleDefault" aria-controls="navbarsExampleDefault"
      aria-expanded="false" aria-label="Toggle navigation">
      <span class="navbar-toggler-icon"></span>
    </button>

    <div class="navbar-collapse">
      <ul class="navbar-nav mr-auto">
        <li class="nav-item active">
          <a class="nav-link" href="#" i18n>My Work</a>
        </li>
        <li *ngIf="isAdmin" ngbDropdown>
          <button class="btn btn-outline-primary" ngbDropdownToggle i18n>Admin</button>
          <div ngbDropdownMenu class="dropdown-menu" aria-labelledby="dropdown01">
            <a class="dropdown-item" href="#">Action</a>
            <a class="dropdown-item" href="#">Another action</a>
            <a class="dropdown-item" href="#">Something else here</a>
          </div>
        </li>
      </ul>
      <form class="form-inline">
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
  </main>
  `
})
export class ShellComponent implements OnInit {
  public get username(): string {
    return this._authService.username;
  }

  public get isAdmin(): boolean {
    return this._authService.username === 'admin';
  }

  public constructor(
    private _authService: AuthService
  ) { }

  public ngOnInit(): void {
  }
}
