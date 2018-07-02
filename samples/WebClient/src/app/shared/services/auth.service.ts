import { Injectable, EventEmitter } from '@angular/core';

import { ServicesModule } from './services.module';
import { StorageService } from './storage.service';

export interface IToken {
  access_token: string;
  expires_in: number;
  token_type: string;
}

@Injectable({
  providedIn: ServicesModule
})
export class AuthService {
  private static ANONYMOUS = 'anonymous';
  private static TOKEN_KEY = 'token_key';

  private _token: IToken;
  private _username: string = AuthService.ANONYMOUS;
  private _claims: Array<string> = new Array<string>();

  public onAuthenticated: EventEmitter<boolean> = new EventEmitter<boolean>();

  public get isAuthenticated(): boolean {
    // TODO: check also expiration!

    return this._token
      ? true
      : false;
  }

  public get username(): string {
    return this._username;
  }

  public get claims(): Array<string> {
    return this._claims;
  }

  public constructor(
    private _storage: StorageService
  ) {
    this.tryAuthenticate();
  }

  public getToken(): string {
    if (this._token) {
      return this._token.access_token;
    }

    const storedToken = this._storage
      .getItem<IToken>(AuthService.TOKEN_KEY);
    if (storedToken) {
      this.authenticate(storedToken);
      return this._token.access_token;
    }
  }

  public authenticate(token: IToken): void {
    if (token) {
      this._token = token;
      this._storage.setItem(AuthService.TOKEN_KEY, this._token);

      this.setProperties();

      this.onAuthenticated.next(this.isAuthenticated);
    }
  }

  public logout(): void {
    this._storage.removeItem(AuthService.TOKEN_KEY);
    delete this._token;

    this.onAuthenticated.next(this.isAuthenticated);
  }

  public hasClaim(claim: string): boolean {
    if (!this.isAuthenticated) {
      return true; // no checks for offline usage!
    }

    return this._claims.some((r => r === claim));
  }

  public tryAuthenticate(): void {
    const token = this._storage.getItem<IToken>(AuthService.TOKEN_KEY);
    if (token) {
      // TODO: check expiration!!!

      this.authenticate(token);
    }
  }

  public setClaims(claims: string[]): void {
    this._claims = claims;
  }

  private setProperties(): void {
    if (this._token) {
      const jwt = JSON.parse(window.atob(this._token.access_token.split('.')[1]));

      this._username = jwt.name;

      // TODO: fetch claims?!
      this._claims = Array.isArray(jwt.role)
        ? jwt.role
        : [jwt.role];

      return;
    }

    this._username = AuthService.ANONYMOUS;
    this._claims = new Array<string>();
  }
}
