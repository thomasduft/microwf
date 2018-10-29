import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  HostBinding
} from '@angular/core';

import {
  WorkflowSearchModel,
  WorkflowSearchSlot
} from './models';

@Component({
  selector: 'tw-workflow-search',
  template: `
  <button type="button" (click)="toggle()">
    <tw-icon name="filter"></tw-icon>
    Filter
  </button>
  <tw-formdef *ngIf="showFilter"
    #formDef
    [showSave]="true"
    [saveTitle]="'Apply'"
    [key]="key"
    [viewModel]="viewModel"
    (submitted)="search($event)"
    (resetted)="cancel()">
  </tw-formdef>
  `
})
export class WorkflowSearchComponent implements OnInit {
  public showFilter = false;

  public key = WorkflowSearchSlot.KEY;
  public viewModel: WorkflowSearchModel;

  @HostBinding('class')
  public class = 'workflow__search';

  @Output()
  public searchClicked: EventEmitter<WorkflowSearchModel>
    = new EventEmitter<WorkflowSearchModel>();

  public ngOnInit(): void {
    this.viewModel = {
      type: null,
      correlationId: null,
      assignee: null
    };
  }

  public toggle(): void {
    this.showFilter = !this.showFilter;
  }

  public search(vm: WorkflowSearchModel): void {
    this.searchClicked.emit(vm);
  }
}
