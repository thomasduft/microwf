import { Subscription } from 'rxjs';

import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { AutoUnsubscribe } from '../shared/services/autoUnsubscribe';
import { FormdefComponent } from '../shared/formdef/index';
import { WorkflowResult, TriggerInfo, NoWorkflowResult } from '../workflow/index';

import { HolidayService } from './holiday.service';
import {
  Holiday,
  ApplyHolidayDetailSlot,
  ApplyHoliday,
  ApproveHolidayDetailSlot,
  ApproveHoliday
} from './models';

// TODO: component and route per state/viewmodel?

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
      <tw-formdef *ngIf="viewModel"
        #formDef
        [key]="key"
        [viewModel]="viewModel"
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
  public viewModel: any;
  public triggerInfo: TriggerInfo;

  @ViewChild('formDef')
  public formDef: FormdefComponent;

  public constructor(
    private _route: ActivatedRoute,
    private _router: Router,
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

    if (trigger === 'apply') {
      this._service.apply(this.formDef.formValue)
        .subscribe((result: WorkflowResult<NoWorkflowResult>) => {
          if (result.triggerInfo.succeeded) {
            this._router.navigate(['dispatch', result.viewModel.assignee, 'holiday']);
          }
        });
    }

    if (trigger === 'approve') {
      this._service.approve(this.formDef.formValue)
        .subscribe((result: WorkflowResult<NoWorkflowResult>) => {
          if (result.triggerInfo.succeeded) {
            this._router.navigate(['dispatch', result.viewModel.assignee, 'holiday']);
          }
        });
    }

    if (trigger === 'reject') {
      this._service.reject(this.formDef.formValue)
        .subscribe((result: WorkflowResult<NoWorkflowResult>) => {
          if (result.triggerInfo.succeeded) {
            this._router.navigate(['dispatch', result.viewModel.assignee, 'holiday']);
          }
        });
    }
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
        this.key = ApproveHolidayDetailSlot.KEY;
        const vm: ApproveHoliday = {
          id: Number(id),
          approved: false,
          message: ''
        };

        this.viewModel = vm;
        this.triggerInfo = result.triggerInfo;
      });
  }

  private create(): void {
    this._holiday$ = this._service.create()
      .subscribe((result: WorkflowResult<ApplyHoliday>) => {
        this.viewModel = result.viewModel;
        this.triggerInfo = result.triggerInfo;
      });
  }
}
