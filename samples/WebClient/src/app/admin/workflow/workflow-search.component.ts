import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  HostBinding,
  Input
} from '@angular/core';

import { WorkflowSearchModel } from '../../workflow/models';
import { WorkflowSearchSlot } from './models';

@Component({
  selector: 'tw-workflow-search',
  template: `
  <tw-formdef
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

  @Input()
  public viewModel: WorkflowSearchModel;

  @Output()
  public searchClicked: EventEmitter<WorkflowSearchModel>
    = new EventEmitter<WorkflowSearchModel>();

  @Output()
  public resettedClicked: EventEmitter<any>
    = new EventEmitter<any>();

  @HostBinding('class')
  public class = 'workflow__search';

  public ngOnInit(): void {
    if (this.viewModel === undefined) {
      this.viewModel = this.empty;
    }
  }

  public search(vm: WorkflowSearchModel): void {
    this.searchClicked.emit(vm);
  }

  public reset(): void {
    this.resettedClicked.emit();
  }
}
