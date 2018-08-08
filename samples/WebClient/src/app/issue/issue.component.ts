import { Subscription } from 'rxjs';

import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { AutoUnsubscribe } from '../shared/services/autoUnsubscribe';
import { AuthService } from '../shared/services/auth.service';
import { FormdefComponent } from '../shared/formdef/index';
import { WorkflowResult, TriggerInfo, NoWorkflowResult } from '../workflow/index';

import { IssueService } from './issue.service';
import { IssueDetailSlot, Issue, IssueViewmodel } from './models';

@AutoUnsubscribe
@Component({
  selector: 'tw-issue',
  providers: [IssueService],
  template: `
  <div class="row">
    <div class="col-sm">
      <tw-trigger-info *ngIf="!isNew && formDef"
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
        [showSave]="isNew"
        [showCancel]="isNew"
        [key]="key"
        [viewModel]="viewModel"
        (submitted)="submitted($event)"
        (resetted)="cancel()">
      </tw-formdef>
    </div>
  </div>
  `
})
export class IssueComponent implements OnInit {
  private _routeParams$: Subscription;
  private _issue$: Subscription;

  public key = IssueDetailSlot.KEY;
  public viewModel: any;
  public entity: Issue;
  public canTrigger = false;
  public triggerInfo: TriggerInfo;

  public isNew = false;

  @ViewChild('formDef')
  public formDef: FormdefComponent;

  public constructor(
    private _route: ActivatedRoute,
    private _router: Router,
    private _auth: AuthService,
    private _service: IssueService
  ) { }

  public ngOnInit(): void {
    this._routeParams$ = this._route.params
      .subscribe((params: Params) => {
        if (params.id) {
          this.init(params.id);
        }
      });
  }

  public submitted(viewModel: IssueViewmodel): void {
    this._issue$ = this._service.save(viewModel)
      .subscribe((id: number) => {
        if (id > 0) {
          this._router.navigate(['issue']);
        }
      });
  }

  public cancel(): void {
    this._router.navigate(['issue']);
  }

  public triggerClicked(trigger: string): void {
    const model: IssueViewmodel = {
      id: this.viewModel.id,
      trigger: trigger,
      assignee: this.viewModel.assignee,
      title: this.viewModel.title,
      description: this.viewModel.description
    };

    this._issue$ = this._service.process(model)
      .subscribe((result: WorkflowResult<Issue, NoWorkflowResult>) => {
        if (result.triggerInfo.succeeded
          && result.viewModel.assignee !== this._auth.username) {
          this._router.navigate(['dispatch', result.viewModel.assignee, 'issue']);
        } else {
          this.viewModel = result.entity;
          this.triggerInfo = result.triggerInfo;
          this.entity = result.entity;
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
    this._issue$ = this._service.get(id)
      .subscribe((result: WorkflowResult<null, Issue>) => {
        this.key = IssueDetailSlot.KEY;

        this.viewModel = result.viewModel;
        this.triggerInfo = result.triggerInfo;
        this.entity = result.entity;
      });
  }

  private create(): void {
    this._issue$ = this._service.create()
      .subscribe((result: WorkflowResult<Issue, IssueViewmodel>) => {
        this.viewModel = result.viewModel;
        this.triggerInfo = result.triggerInfo;
        this.entity = result.entity;

        this.isNew = true;
      });
  }
}
