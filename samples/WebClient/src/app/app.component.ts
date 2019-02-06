import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AuthService } from './shared/services/auth.service';

@Component({
  selector: 'tw-root',
  template: `
  <router-outlet></router-outlet>
  `
})
export class AppComponent implements OnInit {
  public loading = true;

  public constructor(
    private _router: Router,
    private _authService: AuthService
  ) { }

  public ngOnInit(): void {
    this._authService.onAuthenticated
      .subscribe((isOuthenticated: boolean) => {
        if (isOuthenticated) {
          this._router.navigate(['home']);
        } else {
          this._router.navigate(['login']);
        }
      });
  }
}
