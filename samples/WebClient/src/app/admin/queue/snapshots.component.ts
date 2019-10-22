import { Subscription } from 'rxjs';

import { Component, OnInit, HostBinding, ViewChild } from '@angular/core';

import { AutoUnsubscribe } from '../../shared/services/autoUnsubscribe';
import { ListComponent } from '../../shared/list/list.component';

import { PagingModel } from '../../shared/services/models';
import { JobQueueService, QueueItem } from './../../workflow/index';

@AutoUnsubscribe
@Component({
  selector: 'tw-queue-dashboard',
  providers: [JobQueueService],
  template: `
  <div class="pane__left">
    <tw-list [rows]="snapshotItems">
      <tw-header>
        <h3 i18n>Snapshots</h3>
      </tw-header>
      <ng-template let-queueItem twTemplate="queueItem-item">
        <tw-queue-list-item
          [queueItem]="queueItem">
        </tw-queue-list-item>
      </ng-template>
    </tw-list>
  </div>
  <div class="pane__main">
    <router-outlet></router-outlet>
  </div>`
})
export class SnapshotsComponent implements OnInit {
  private snapshotItems$: Subscription;
  private upcommingItems$: Subscription;

  public snapshotItems: Array<QueueItem> = [];
  public upcommingItems: Array<QueueItem> = [];
  public failedItems: Array<QueueItem> = [];

  private upcommingPage: PagingModel = PagingModel.create();
  private failedPage: PagingModel = PagingModel.create();

  @HostBinding('class')
  public workspace = 'pane';

  @ViewChild(ListComponent, { static: false })
  public upcommingList: ListComponent;

  @ViewChild(ListComponent, { static: false })
  public failedList: ListComponent;

  public constructor(
    private _service: JobQueueService
  ) { }

  public ngOnInit(): void {
    this.loadSnapshot();
    this.loadUpcommings(this.upcommingPage);
  }

  private loadSnapshot(): void {
    this.snapshotItems$ = this._service.snapshot()
      .subscribe((response: Array<QueueItem>) => {
        this.snapshotItems = response;
      });
  }

  private loadUpcommings(pagingModel: PagingModel): void {
    this.upcommingItems$ = this._service.upcommings(pagingModel)
      .subscribe((response: any) => {
        const xPagination = JSON.parse(response.headers.get('X-Pagination'));
        this.upcommingPage = PagingModel.fromResponse(xPagination);

        this.upcommingList.attachRows(response.body);
      });
  }
}
