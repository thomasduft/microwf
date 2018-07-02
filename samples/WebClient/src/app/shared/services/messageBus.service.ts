import { Injectable } from '@angular/core';

import { ServicesModule } from './services.module';
import { MessageBase, IMessageSubscriber } from './models';

@Injectable({
  providedIn: ServicesModule
})
export class MessageBus {
  private _subscribers: { [id: number]: IMessageSubscriber<MessageBase>; };

  public constructor() {
    this._subscribers = {};
  }

  public subsribe<T extends MessageBase>(subscriber: IMessageSubscriber<T>): number {
    const id = new Date().valueOf();
    this._subscribers[id] = subscriber;
    return id;
  }

  public unsubscribe(id: number): void {
    delete this._subscribers[id];
  }

  public publish<T extends MessageBase>(message: T): void {
    const subscribersForMessage = this.getSubscribersForMessage<T>(message);

    if (subscribersForMessage.length === 0) {
      console.log(`No subscribers for ${message.getType()} registred!`);
      return;
    }

    subscribersForMessage.forEach((subscriberForMessage: IMessageSubscriber<T>) => {
      subscriberForMessage.onMessage(message);
    });
  }

  private getSubscribersForMessage<T extends MessageBase>(message: T): Array<IMessageSubscriber<T>> {
    const subscribers = new Array<IMessageSubscriber<T>>();

    for (const key in this._subscribers) {
      if (this._subscribers.hasOwnProperty(key)) {
        const subscriber = this._subscribers[key];
        if (subscriber.getType() === message.getType()) {
          subscribers.push(subscriber);
        }
      }
    }

    return subscribers;
  }
}
