import { Subscription } from 'rxjs';

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
  <h2 i18n>My work</h2>
  <a class="btn btn-primary" [routerLink]="['detail/new']" i18n>New</a>
  <div class="table-responsive-md">
    <table class="table table-hover">
      <thead>
        <tr>
          <th scope="col" i18n>State</th>
          <th scope="col" i18n>Requestor</th>
          <th scope="col" i18n>From</th>
          <th scope="col" i18n>To</th>
          <th scope="col" i18n>Action</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let holiday of myWork">
          <td>{{ holiday.state }}</td>
          <td>{{ holiday.requestor }}</td>
          <td>{{ holiday.from | date }}</td>
          <td>{{ holiday.to | date }}</td>
          <td>
            <a [routerLink]="['detail', holiday.id]" i18n>open</a>
          </td>
        </tr>
      </tbody>
    </table>
  </div>`
})
export class HolidayDashboardComponent implements OnInit {
  private _myWork$: Subscription;

  public myWork: Array<Holiday> = [];

  public constructor(
    private _router: Router,
    private _service: HolidayService
  ) { }

  public ngOnInit(): void {
    this._myWork$ = this._service.myWork()
      .subscribe((myWork: Array<Holiday>) => {
        this.myWork = myWork;
      });
  }

  public create(): void {
    this._router.navigate(['./detail/new']);
  }
}
