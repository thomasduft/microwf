import { Subscription } from 'rxjs';

import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { AutoUnsubscribe } from '../shared/services/autoUnsubscribe';
import { AuthService } from '../shared/services/auth.service';
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

@AutoUnsubscribe
@Component({
  selector: 'tw-holiday',
  providers: [HolidayService],
  template: `
  <div class="row">
    <div class="col-sm">
      <tw-trigger-info *ngIf="formDef"
        [canTrigger]="formDef.formIsValid"
        [triggerInfo]="triggerInfo"
        (trigger)="triggerClicked($event)">
      </tw-trigger-info>
    </div>
  </div>

  <div class="row">
    <div class="col-sm">
      <tw-formdef
        #formDef
        [key]="key"
        [viewModel]="viewModel"
        (submitted)="submitted($event)">
      </tw-formdef>
    </div>
  </div>

  <div class="row" *ngIf="entity && entity.id">
    <div class="col-sm">
      <p>
        <b>{{ entity.requester }}</b> applied for holiday between 
        <b>{{ entity.from | date }}</b> and <b>{{ entity.to | date }}</b>.
      </p>
      <h5 i18n>Messages</h5>
      <div class="table-responsive-md" 
        *ngIf="entity.messages && entity.messages.length > 0">
        <table class="table table-hover">
          <thead>
            <tr>
              <th scope="col" i18n>Author</th>
              <th scope="col" i18n>Message</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let message of entity.messages">
              <td>{{ message.author }}</td>
              <td>{{ message.message }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
  `
})
export class HolidayComponent implements OnInit {
  private _routeParams$: Subscription;
  private _holiday$: Subscription;

  public key = ApplyHolidayDetailSlot.KEY;
  public viewModel: any;
  public entity: Holiday;
  public canTrigger = false;
  public triggerInfo: TriggerInfo;

  @ViewChild('formDef')
  public formDef: FormdefComponent;

  public constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    private _auth: AuthService,
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
    if (trigger === 'apply') {
      this._service.apply(this.formDef.formValue)
        .subscribe((result: WorkflowResult<Holiday, NoWorkflowResult>) => {
          if (result.triggerInfo.succeeded
            && result.viewModel.assignee !== this._auth.username) {
            this._router.navigate(['dispatch', result.viewModel.assignee, 'holiday']);
          }
        });
    }

    if (trigger === 'approve') {
      this._service.approve(this.formDef.formValue)
        .subscribe((result: WorkflowResult<Holiday, NoWorkflowResult>) => {
          if (result.triggerInfo.succeeded
            && result.viewModel.assignee !== this._auth.username) {
            this._router.navigate(['dispatch', result.viewModel.assignee, 'holiday']);
          }
        });
    }

    if (trigger === 'reject') {
      this._service.reject(this.formDef.formValue)
        .subscribe((result: WorkflowResult<Holiday, NoWorkflowResult>) => {
          if (result.triggerInfo.succeeded
            && result.viewModel.assignee !== this._auth.username) {
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
      .subscribe((result: WorkflowResult<null, Holiday>) => {
        this.key = ApproveHolidayDetailSlot.KEY;

        this.viewModel = result.viewModel;
        this.triggerInfo = result.triggerInfo;
        this.entity = result.entity;
      });
  }

  private create(): void {
    this._holiday$ = this._service.create()
      .subscribe((result: WorkflowResult<Holiday, ApplyHoliday>) => {
        this.viewModel = result.viewModel;
        this.triggerInfo = result.triggerInfo;
        this.entity = result.entity;
      });
  }
}
