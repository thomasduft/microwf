import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'tw-queue-dashboard',
  template: `
  <div class="pane__left">
    <div class="sidebar__content">
      <ul>
        <li routerLinkActive="active" class="menu__item menu__item--bg-dark">
          <a routerLink="snapshots" i18n>Snapshots</a>
        </li>
        <li routerLinkActive="active" class="menu__item menu__item--bg-dark">
          <a routerLink="upcommings" i18n>Upcommings</a>
        </li>
        <li routerLinkActive="active" class="menu__item menu__item--bg-dark">
          <a routerLink="failed" i18n>Failed</a>
        </li>
      </ul>
    </div>
  </div>
  <div class="pane__main">
    <router-outlet></router-outlet>
  </div>`
})
export class QueueDashboardComponent {
  @HostBinding('class')
  public workspace = 'pane';
}
