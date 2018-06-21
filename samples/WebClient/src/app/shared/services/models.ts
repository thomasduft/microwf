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
  providedIn: ServicesModule
})
export abstract class ClaimGuardBase implements CanActivate, CanActivateChild {
  protected readonly abstract claim: string;

  public constructor(
    private _authService: AuthService,
    private _router: Router
  ) { }

  public canActivate(): Observable<boolean> | Promise<boolean> | boolean {
    const can = this._authService.hasClaim(this.claim);

    if (!can) {
      this._router.navigate(['401', this.claim]);
    }

    return can;
  }

  public canActivateChild(): Observable<boolean> | Promise<boolean> | boolean {
    return this.canActivate();
  }
}

@Injectable({
  providedIn: ServicesModule
})
export class AdministratorClaimGuard extends ClaimGuardBase {
  public static CLAIM_NAME = 'Administrator';
  protected claim = AdministratorClaimGuard.CLAIM_NAME;
}
