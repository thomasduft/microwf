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
  private empty: WorkflowSearchModel = {
    type: null,
    correlationId: null,
    assignee: null
  };

  public showFilter = false;

  public key = WorkflowSearchSlot.KEY;
  public viewModel: WorkflowSearchModel;


  @HostBinding('class')
  public class = 'workflow__search';

  @Output()
  public searchClicked: EventEmitter<WorkflowSearchModel>
    = new EventEmitter<WorkflowSearchModel>();

  public ngOnInit(): void {
    this.viewModel = this.empty;
  }

  public toggle(): void {
    this.showFilter = !this.showFilter;

    if (!this.showFilter) {
      this.search(this.empty);
    }
  }

  public search(vm: WorkflowSearchModel): void {
    this.searchClicked.emit(vm);
  }
}
