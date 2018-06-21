import { Subscription } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { AutoUnsubscribe } from '../shared/services/autoUnsubscribe';
import { HolidayService } from './holiday.service';
import { Holiday } from './models';

@AutoUnsubscribe
@Component({
  selector: 'tw-holiday',
  providers: [HolidayService],
  template: `
  <p>
    holiday works!
  </p>`
})
export class HolidayComponent implements OnInit {
  private _routeParams$: Subscription;
  private _holiday$: Subscription;

  public holiday: Holiday;

  public constructor(
    private _route: ActivatedRoute,
    private _service: HolidayService
  ) { }

  public ngOnInit(): void {
    this._routeParams$ = this._route.params
      .subscribe((params: Params) => {
        if (params.id) {
          this.init(params.id);
        }
      });
  }

  private init(id?: string): void {
    if (id !== 'new') {
      this.load(id.toString());
    } else {
      this.create();
    }
  }

  private load(id: string): void {
    this._holiday$ = this._service.get(id)
      .subscribe((h: Holiday) => {
        this.holiday = h;
      });
  }

  private create(): void {
    this._holiday$ = this._service.create()
      .subscribe((h: Holiday) => {
        this.holiday = h;
      });
  }
}
