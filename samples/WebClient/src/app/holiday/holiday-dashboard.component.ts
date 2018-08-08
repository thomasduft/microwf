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
  <h1 i18n>Holiday</h1>
  <hr />
  <div class="btn-group float-right">
    <a class="btn btn-primary" [routerLink]="['detail/new']" i18n>New</a>
    <button type="button" class="btn btn-secondary" (click)="reload()">reload</button>
  </div>
  <h2 i18n>My work</h2>
  <div class="table-responsive-md">
    <table class="table table-hover">
      <thead>
        <tr>
          <th scope="col" i18n>State</th>
          <th scope="col" i18n>Requester</th>
          <th scope="col" i18n>From</th>
          <th scope="col" i18n>To</th>
          <th scope="col" i18n>Action</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let holiday of myWork$ | async">
          <td>{{ holiday?.state }}</td>
          <td>{{ holiday?.requester }}</td>
          <td>{{ holiday?.from | date }}</td>
          <td>{{ holiday?.to | date }}</td>
          <td>
            <a [routerLink]="['detail', holiday?.id]" i18n>open</a>
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
