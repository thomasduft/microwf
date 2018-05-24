import { Component } from '@angular/core';
import {
  Router,
  Event,
  NavigationStart,
  NavigationEnd,
  NavigationError,
  NavigationCancel
} from '@angular/router';

@Component({
  selector: 'tw-root',
  styleUrls: ['./app.component.css'],
  template: `
  <span class="glyphicon glyphicon-refresh glyphicon-spin spinner" *ngIf="loading"></span>
  <router-outlet></router-outlet>
  `
})
export class AppComponent {
  public loading = true;

  public constructor(
    private router: Router
  ) {
    router.events.subscribe((routerEvent: Event) => {
      this.checkRouterEvent(routerEvent);
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
