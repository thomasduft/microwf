import { Subscription } from 'rxjs';

import { Component, OnInit, HostBinding, ViewChild } from '@angular/core';

import { AutoUnsubscribe } from '../../shared/services/autoUnsubscribe';
import { ListComponent } from '../../shared/list/list.component';

import { PagingModel } from '../../shared/services/models';
import { JobQueueService, QueueItem } from './../../workflow/index';

@AutoUnsubscribe
@Component({
  selector: 'tw-upcommings',
  providers: [JobQueueService],
  template: `
  <div class="pane__main"
    twScroller
    [offset]="40"
    (scrollEnd)="loadNextPage()">
    <tw-list [rows]="upcommingItems">
    <tw-header>
        <div class="button__bar">
          <button type="button" (click)="refresh()">
            <tw-icon name="refresh"></tw-icon>
          </button>
        </div>
      </tw-header>
      <ng-template let-queueItem twTemplate="upcomming-item">
        <tw-queue-list-item
          [queueItem]="queueItem">
        </tw-queue-list-item>
      </ng-template>
    </tw-list>
  </div>`
})

export class UpcommingsComponent implements OnInit {
  private upcommingItems$: Subscription;

  private upcommingPage: PagingModel = PagingModel.create();

  public upcommingItems: Array<QueueItem> = [];

  @ViewChild(ListComponent)
  public upcommingList: ListComponent;

  @HostBinding('class')
  public style = 'pane';

  public constructor(
    private _service: JobQueueService
  ) { }

  public ngOnInit(): void {
    this.loadData();
  }

  public loadNextPage(): void {
    if (this.upcommingPage.totalPages - 1 > this.upcommingPage.pageIndex) {
      this.loadUpcommings(PagingModel.createNextPage(this.upcommingPage.pageIndex, 50));
    }
  }

  public refresh(): void {
    this.upcommingList.rows = [];
    this.upcommingPage = PagingModel.create();
    this.ngOnInit();
  }

  private loadData(): void {
    this.loadUpcommings(this.upcommingPage);
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
