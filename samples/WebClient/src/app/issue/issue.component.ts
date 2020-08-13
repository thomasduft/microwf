import { Subscription } from 'rxjs';

import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { AutoUnsubscribe } from '../shared/services/autoUnsubscribe';
import { UserService } from '../shared/services/user.service';
import { FormdefComponent, FormdefRegistry } from '../shared/formdef/index';
import { WorkflowResult, TriggerInfo, AssigneeWorkflowResult } from '../workflow/index';

import { IssueService } from './issue.service';
import { IssueDetailSlot, Issue, IssueViewmodel } from './models';

@AutoUnsubscribe
@Component({
  selector: 'tw-issue',
  providers: [IssueService],
  template: `
  <div>
    <tw-trigger-info *ngIf="!isNew && formDef"
      [canTrigger]="formDef.formIsValid"
      [triggerInfo]="triggerInfo"
      (trigger)="triggerClicked($event)">
    </tw-trigger-info>
  </div>
  <div>
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
  `
})
export class IssueComponent implements OnInit {
  private _routeParams$: Subscription;
  private _issue$: Subscription;
  private _assignees$: Subscription;

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
    private _user: UserService,
    private _service: IssueService,
    private _slotRegistry: FormdefRegistry
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
          this.back();
        }
      });
  }

  public cancel(): void {
    this.back();
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
      .subscribe((result: WorkflowResult<Issue, AssigneeWorkflowResult>) => {
        if (result.triggerInfo.succeeded
          && result.viewModel.assignee !== this._user.userName) {
          this._router.navigate(['dispatch', result.viewModel.assignee, 'issue']);
        } else {
          this.viewModel = result.entity;
          this.triggerInfo = result.triggerInfo;
          this.entity = result.entity;
        }
      });
  }

  private back() {
    this._router.navigate(['issue']);
  }

  private init(id?: string): void {
    this._assignees$ = this._service.assignees()
      .subscribe((assignees: Array<string>) => {
        this._slotRegistry.register(new IssueDetailSlot(assignees));

        if (id !== 'new') {
          this.load(id.toString());
        } else {
          this.create();
        }
      });
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
