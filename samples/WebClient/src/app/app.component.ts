import { Component, OnInit } from '@angular/core';
import {
  Router,
  Event,
  NavigationStart,
  NavigationEnd,
  NavigationError,
  NavigationCancel
} from '@angular/router';

import { AuthService } from './shared/services/auth.service';
import { WorkflowAreaRegistry } from './shared/services/workflow-area-registry.service';

@Component({
  selector: 'tw-root',
  styleUrls: ['./app.component.css'],
  template: `
  <span class="glyphicon glyphicon-refresh glyphicon-spin spinner"
        *ngIf="loading">
  </span>
  <router-outlet></router-outlet>
  `
})
export class AppComponent implements OnInit {
  public loading = true;

  public constructor(
    private _router: Router,
    private _authService: AuthService,
    private _workflowAreaRegistry: WorkflowAreaRegistry
  ) {
    _router.events.subscribe((routerEvent: Event) => {
      this.checkRouterEvent(routerEvent);
    });
  }

  public ngOnInit(): void {
    this._authService.onAuthenticated
      .subscribe((isOuthenticated: boolean) => {
        if (isOuthenticated) {
          this._router.navigate(['home']);
        } else {
          this._workflowAreaRegistry.clear();
          this._router.navigate(['login']);
        }
    });
  }

  private checkRouterEvent(routerEvent: Event): void {
    if (routerEvent instanceof NavigationStart) {
      this.loading = true;
    }

    if (
      routerEvent instanceof NavigationEnd ||
      routerEvent instanceof NavigationCancel ||
      routerEvent instanceof NavigationError
    ) {
      this.loading = false;
    }
  }
}
