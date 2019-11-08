import { Subscription, interval } from 'rxjs';

import { Component, OnInit, HostBinding, ViewChild } from '@angular/core';

import { AutoUnsubscribe } from '../../shared/services/autoUnsubscribe';
import { ListComponent } from '../../shared/list/list.component';

import { PagingModel } from '../../shared/services/models';
import { JobQueueService, QueueItem } from './../../workflow/index';

@AutoUnsubscribe
@Component({
  selector: 'tw-snapshots',
  providers: [JobQueueService],
  template: `
  <div class="pane__main">
    <tw-list [rows]="snapshotItems">
      <ng-template let-queueItem twTemplate="queueItem-item">
        <tw-queue-list-item
          [queueItem]="queueItem">
        </tw-queue-list-item>
      </ng-template>
    </tw-list>
  </div>`
})
export class SnapshotsComponent implements OnInit {
  private snapshotItems$: Subscription;
  private upcommingItems$: Subscription;
  private intervall$: Subscription;

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
    this.intervall$ = interval(5000)
      .subscribe(() => { this.loadData(); });
  }

  private loadData(): void {
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
