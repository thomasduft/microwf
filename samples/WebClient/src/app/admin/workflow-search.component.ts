import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  HostBinding
} from '@angular/core';

import { WorkflowSearchModel } from '../workflow/models';
import { WorkflowSearchSlot } from './models';

@Component({
  selector: 'tw-workflow-search',
  template: `
  <tw-formdef
    #formDef
    [showSave]="true"
    [showCancel]="true"
    [saveTitle]="'Apply'"
    [key]="key"
    [viewModel]="viewModel"
    (submitted)="search($event)"
    (resetted)="reset()">
  </tw-formdef>
  `
})
export class WorkflowSearchComponent implements OnInit {
  private empty: WorkflowSearchModel = {
    type: null,
    correlationId: null,
    assignee: null
  };

  public key = WorkflowSearchSlot.KEY;
  public viewModel: WorkflowSearchModel;

  @HostBinding('class')
  public class = 'workflow__search';

  @Output()
  public searchClicked: EventEmitter<WorkflowSearchModel>
    = new EventEmitter<WorkflowSearchModel>();

  @Output()
  public resettedClicked: EventEmitter<any>
    = new EventEmitter<any>();

  public ngOnInit(): void {
    this.viewModel = this.empty;
  }

  public search(vm: WorkflowSearchModel): void {
    this.searchClicked.emit(vm);
  }

  public reset(): void {
    this.resettedClicked.emit();
  }
}
