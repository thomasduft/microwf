import { Subject, BehaviorSubject } from 'rxjs';

import { Injectable } from '@angular/core';

import { MessageBus } from './../../../shared/services/messageBus.service';
import { IMessageSubscriber } from './../../../shared/services/models';
import { StatusMessage } from './models';

@Injectable()
export class StatusBarService implements IMessageSubscriber<StatusMessage> {
  private _status: Subject<StatusMessage> = new BehaviorSubject<StatusMessage>(null);

  public get status(): Subject<StatusMessage> {
    return this._status;
  }

  public constructor(
    private _messageBus: MessageBus
  ) {
    this._messageBus.subscribe(this);
  }

  public onMessage(message: StatusMessage): void {
    this.setStatus(message);
  }

  public getType(): string {
    return StatusMessage.KEY;
  }

  private setStatus(message: StatusMessage): void {
    this._status.next(message);
  }
}
