import { Observable } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AutoUnsubscribe } from './../shared/services/autoUnsubscribe';

import { HolidayService } from './holiday.service';
import { Holiday } from './models';

@AutoUnsubscribe
@Component({
  selector: 'tw-holiday-dashboard',
  providers: [HolidayService],
  template: `
  <div class="btn-group float-right">
    <a class="btn btn-primary" [routerLink]="['detail/new']">
      <tw-icon name="plus"></tw-icon>
    </a>
    <button type="button" class="btn btn-secondary" (click)="reload()">
      <tw-icon name="refresh"></tw-icon>
    </button>
  </div>
  <h1 i18n>Holidays</h1>
  <div class="table-responsive-md">
    <table class="table table-hover">
      <thead>
        <tr>
          <th scope="col" i18n>State</th>
          <th scope="col" i18n>Requester</th>
          <th scope="col" i18n>From</th>
          <th scope="col" i18n>To</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let holiday of myWork$ | async">
          <td>{{ holiday?.state }}</td>
          <td>{{ holiday?.requester }}</td>
          <td>{{ holiday?.from | date }}</td>
          <td>{{ holiday?.to | date }}</td>
          <td>
            <a [routerLink]="['detail', holiday?.id]" i18n>
              <tw-icon name="arrow-right"></tw-icon>
            </a>
          </td>
        </tr>
      </tbody>
    </table>
  </div>`
})
export class HolidayDashboardComponent implements OnInit {
  public myWork$: Observable<Array<Holiday>>;

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
