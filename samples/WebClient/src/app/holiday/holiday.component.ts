import { Subscription } from 'rxjs';

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { AutoUnsubscribe } from '../shared/services/autoUnsubscribe';
import { WorkflowResult, TriggerInfo } from '../workflow/index';

import { HolidayService } from './holiday.service';
import { Holiday, ApplyHolidayDetailSlot } from './models';

@AutoUnsubscribe
@Component({
  selector: 'tw-holiday',
  providers: [HolidayService],
  template: `
  <div class="row">
    <div class="col-sm">
      <tw-trigger-info
        [triggerInfo]="triggerInfo"
        (trigger)="triggerClicked($event)">
      </tw-trigger-info>
    </div>
  </div>

  <div class="row">
    <div class="col-sm">
      <tw-formdef *ngIf="holiday"
        [key]="key"
        [viewModel]="holiday"
        (submitted)="submitted($event)">
      </tw-formdef>
    </div>
  </div>
  `
})
export class HolidayComponent implements OnInit {
  private _routeParams$: Subscription;
  private _holiday$: Subscription;

  public key = ApplyHolidayDetailSlot.KEY;
  public holiday: Holiday;
  public triggerInfo: TriggerInfo;

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

  public submitted(viewModel: Holiday): void {
    console.log(viewModel);
  }

  public triggerClicked(trigger: string): void {
    console.log(trigger);
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
      .subscribe((result: WorkflowResult<Holiday>) => {
        this.holiday = result.viewModel;
        this.triggerInfo = result.triggerInfo;
      });
  }

  private create(): void {
    this._holiday$ = this._service.create()
      .subscribe((result: WorkflowResult<Holiday>) => {
        this.holiday = result.viewModel;
        this.triggerInfo = result.triggerInfo;
      });
  }
}
