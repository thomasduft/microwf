import { Subscription } from 'rxjs';

import { Component, OnInit, HostBinding, ViewChild } from '@angular/core';

import { AutoUnsubscribe } from '../../shared/services/autoUnsubscribe';
import { ListComponent } from '../../shared/list/list.component';

import { PagingModel } from '../../shared/services/models';
import { JobQueueService, QueueItem } from './../../workflow/index';

@AutoUnsubscribe
@Component({
  selector: 'tw-failed',
  providers: [JobQueueService],
  template: `
  <div class="pane__main"
    twScroller
    [offset]="40"
    (scrollEnd)="loadNextPage()">
    <tw-list [rows]="failedItems">
    <tw-header>
        <div class="button__bar">
          <button type="button" (click)="refresh()">
            <tw-icon name="refresh"></tw-icon>
          </button>
        </div>
      </tw-header>
      <ng-template let-queueItem twTemplate="failed-item">
        <tw-queue-list-item
          [queueItem]="queueItem">
        </tw-queue-list-item>
      </ng-template>
    </tw-list>
  </div>`
})
export class FailedComponent implements OnInit {
  private failedItems$: Subscription;

  private failedPage: PagingModel = PagingModel.create();

  public failedItems: Array<QueueItem> = [];

  @ViewChild(ListComponent)
  public failedList: ListComponent;

  @HostBinding('class')
  public style = 'pane';

  public constructor(
    private _service: JobQueueService
  ) { }

  public ngOnInit(): void {
    this.loadData();
  }

  public loadNextPage(): void {
    if (this.failedPage.totalPages - 1 > this.failedPage.pageIndex) {
      this.loadFailed(PagingModel.createNextPage(this.failedPage.pageIndex, 50));
    }
  }

  public refresh(): void {
    this.failedList.rows = [];
    this.failedPage = PagingModel.create();
    this.ngOnInit();
  }

  private loadData(): void {
    this.loadFailed(this.failedPage);
  }

  private loadFailed(pagingModel: PagingModel): void {
    this.failedItems$ = this._service.failed(pagingModel)
      .subscribe((response: any) => {
        const xPagination = JSON.parse(response.headers.get('X-Pagination'));
        this.failedPage = PagingModel.fromResponse(xPagination);

        this.failedList.attachRows(response.body);
      });
  }
}
