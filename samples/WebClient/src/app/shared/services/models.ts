import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';
import { Router, CanActivate, CanActivateChild } from '@angular/router';

import { ServicesModule } from './services.module';
import { AuthService } from './auth.service';

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

export class PagingnModel {
  public selectItemsPerPage: number[] = [5, 10, 25, 100];
  public pageSize = this.selectItemsPerPage[0];
  public pageIndex = 1;
  public allItemsCount = 0;

  public static create(pageIndex: number, pageSize: number): PagingnModel {
    const model = new PagingnModel();
    model.pageIndex = pageIndex;
    model.pageSize = pageSize;

    return model;
  }

  public static createNextPage(pageIndex: number, pageSize: number): PagingnModel {
    const model = new PagingnModel();
    model.pageIndex = pageIndex + 1;
    model.pageSize = pageSize;

    return model;
  }

  public static createPreviousPage(pageIndex: number, pageSize: number): PagingnModel {
    const model = new PagingnModel();
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
  providedIn: ServicesModule
})
export abstract class AuthGuard implements CanActivate, CanActivateChild {
  protected readonly abstract claim: string;

  public constructor(
    private _authService: AuthService,
    private _router: Router
  ) { }

  public canActivate(): Observable<boolean> | Promise<boolean> | boolean {
    const isAuthenticated = this._authService.isAuthenticated;

    if (!isAuthenticated) {
      this._router.navigate(['login']);
    }

    return isAuthenticated;
  }

  public canActivateChild(): Observable<boolean> | Promise<boolean> | boolean {
    return this.canActivate();
  }
}

@Injectable({
  providedIn: 'root'
})
export abstract class ClaimGuardBase implements CanActivate, CanActivateChild {
  protected abstract claim: string;

  public constructor(
    private _authService: AuthService,
    private _router: Router
  ) { }

  public canActivate(): boolean {
    const can = this._authService.hasClaim(this.claim);

    if (!can) {
      this._router.navigate(['login']);
    }

    return can;
  }

  public canActivateChild(): boolean {
    return this.canActivate();
  }
}
