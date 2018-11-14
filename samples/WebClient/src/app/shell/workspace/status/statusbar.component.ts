import { Component, HostBinding, OnInit } from '@angular/core';

import { StatusMessage, StatusLevel } from './models';
import { StatusBarService } from './statusbar.service';

/**
 * StatusBarComponent that is able to show status messages based on the users interaction.
 *
 * @export
 * @class StatusBarComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'tw-statusbar',
  providers: [
    StatusBarService
  ],
  template: `
  <div *ngIf="displayStatusBar()" [class]="statusClass">
    <button type="button" class="close"(click)="close($event)">
      <span>&times;</span>
    </button>
    <button *ngIf="hasAction"
            type="button"
            class="close"
            (click)="action()">
      <span aria-hidden="true">action</span>
    </button>
    <strong>{{ status.title }}: </strong>{{ status.message }}
  </div>`
})
export class StatusBarComponent implements OnInit {
  private _status: StatusMessage;

  @HostBinding('class') workspaceNotification = 'workspace__status';

  public hasAction = false;

  public get status(): StatusMessage {
    return this._status;
  }

  public get statusClass(): string {
    let s = 'success';

    switch (this._status.level) {
      case StatusLevel.Info:
        s = 'info';
        break;
      case StatusLevel.Warning:
        s = 'warning';
        break;
      case StatusLevel.Danger:
        s = 'danger';
        break;
    }

    return `alert alert-${s}`;
  }

  public constructor(
    private _statusBarService: StatusBarService
  ) { }

  public ngOnInit(): void {
    this._statusBarService.status
      .subscribe((status: StatusMessage) => {
        if (!status) {
          // initial StatusMessage will be null!
          return;
        }

        this._status = status;
        this.hasAction = this._status.hasAction;
        this.fadeOut();
      });
  }

  public displayStatusBar(): boolean {
    return this._status && !this._status.viewed;
  }

  public close(): void {
    this._status.viewed = true;
  }

  public action(): void {
    this._status.action();
  }

  private fadeOut(): void {
    if (!this._status
      || this._status.level !== StatusLevel.Success) {
      return;
    }

    setTimeout(() => {
      this._status.viewed = true;
    }, 3000);
  }
}
