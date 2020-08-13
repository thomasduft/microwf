import { Injectable } from '@angular/core';
import { Router, CanActivate, CanActivateChild } from '@angular/router';

import { UserService } from './user.service';

export abstract class MessageBase {
  public abstract getType(): string;
}

export interface IMessageSubscriber<T extends MessageBase> {
  onMessage(message: T): void;
  getType(): string;
}

export interface IdentityError {
  code: string;
  description: string;
}

export interface IdentityResult {
  succeeded: boolean;
  errors: Array<IdentityError>;
}

export class PagingModel {
  public pageSize;
  public pageIndex;
  public totalCount;
  public totalPages;

  public static create(pageIndex = 0, pageSize = 50): PagingModel {
    const model = new PagingModel();
    model.pageIndex = pageIndex;
    model.pageSize = pageSize;

    return model;
  }

  public static fromResponse(xPaginationHeader: any): PagingModel {
    const model = new PagingModel();
    model.totalCount = xPaginationHeader.totalCount;
    model.pageIndex = xPaginationHeader.pageIndex;
    model.pageSize = xPaginationHeader.pageSize;
    model.totalPages = xPaginationHeader.totalPages;

    return model;
  }

  public static createNextPage(pageIndex: number, pageSize = 20): PagingModel {
    const model = new PagingModel();
    model.pageIndex = pageIndex + 1;
    model.pageSize = pageSize;

    return model;
  }

  public static createPreviousPage(pageIndex: number, pageSize: number): PagingModel {
    const model = new PagingModel();
    model.pageIndex = pageIndex - 1;
    model.pageSize = pageSize;

    return model;
  }
}

export class ResponseErrorHandler {
  public static handleError(error: Response): any {
    console.log(error || 'Server error');
    return error;
  }
}

@Injectable({
  providedIn: 'root'
})
export abstract class ClaimGuardBase implements CanActivate, CanActivateChild {
  protected abstract claim: string;

  public constructor(
    private router: Router,
    private user: UserService
  ) { }

  public canActivate(): boolean {
    const can = this.user.hasClaim(this.claim);

    if (!can) {
      this.router.navigate(['login']);
    }

    return can;
  }

  public canActivateChild(): boolean {
    return this.canActivate();
  }
}
