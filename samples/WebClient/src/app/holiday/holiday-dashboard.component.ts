import { Observable } from 'rxjs';

import { Component, OnInit, HostBinding } from '@angular/core';
import { Router } from '@angular/router';

import { AutoUnsubscribe } from './../shared/services/autoUnsubscribe';

import { HolidayService } from './holiday.service';
import { Holiday } from './models';

@AutoUnsubscribe
@Component({
  selector: 'tw-holiday-dashboard',
  providers: [HolidayService],
  template: `
  <div class="pane__left">
    <tw-list #list [rows]="myWork$ | async">
      <tw-header>
        <h3 i18n>Holidays</h3>
        <div class="button__bar">
          <a class="button" [routerLink]="['detail/new']">
            <tw-icon name="plus"></tw-icon>
          </a>
          <button type="button" (click)="reload()">
            <tw-icon name="refresh"></tw-icon>
          </button>
        </div>
      </tw-header>
      <ng-template let-holiday twTemplate="holiday-item">
        <div class="list__item">
          <div [routerLink]="['detail', holiday?.id]" routerLinkActive="active">
            <p>
              <b i18n>Requester:</b><span> {{ holiday?.requester }}</span>
            </p>
            <div class="list__itemDetail">
              <p>
                <b i18n>From:</b> {{ holiday?.from | date }}
                <span class="list_item--right"><b i18n>To:</b> {{ holiday?.to | date }}</span>
              </p>
            </div>
          </div>
        </div>
      </ng-template>
    </tw-list>
  </div>
  <div class="pane__main">
    <router-outlet></router-outlet>
  </div>`
})
export class HolidayDashboardComponent implements OnInit {
  public myWork$: Observable<Array<Holiday>>;

  @HostBinding('class')
  public workspace = 'pane';

  public constructor(
    private _router: Router,
    private _service: HolidayService
  ) { }

  public ngOnInit(): void {
    this.myWork$ = this._service.myWork();
  }

  public create(): void {
    this._router.navigate(['./detail/new']);
  }

  public reload(): void {
    this.ngOnInit();
  }
}
