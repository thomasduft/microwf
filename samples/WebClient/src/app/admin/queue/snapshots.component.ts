import { Subscription, interval } from 'rxjs';

import { Component, OnInit, HostBinding } from '@angular/core';

import { AutoUnsubscribe } from '../../shared/services/autoUnsubscribe';

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
  private intervall$: Subscription;

  public snapshotItems: Array<QueueItem> = [];

  @HostBinding('class')
  public style = 'pane';

  public constructor(
    private _service: JobQueueService
  ) { }

  public ngOnInit(): void {
    this.intervall$ = interval(5000)
      .subscribe(() => { this.loadData(); });
  }

  private loadData(): void {
    this.loadSnapshot();
  }

  private loadSnapshot(): void {
    this.snapshotItems$ = this._service.snapshot()
      .subscribe((response: Array<QueueItem>) => {
        this.snapshotItems = response;
      });
  }
}
