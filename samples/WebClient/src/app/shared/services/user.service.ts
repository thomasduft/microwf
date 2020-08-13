import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private static ANONYMOUS = 'anonymous';

  private username: string = UserService.ANONYMOUS;
  private claims: Array<string> = new Array<string>();

  public authenticated$: EventEmitter<void> = new EventEmitter<void>();

  public get isAuthenticated(): boolean {
    return this.username !== UserService.ANONYMOUS;
  }

  public get userName(): string {
    return this.username;
  }

  public get userClaims(): Array<string> {
    return this.claims;
  }

  public reset(): void {
    this.setProperties();
  }

  public hasClaim(claim: string): boolean {
    if (!this.claims || !claim) {
      return false;
    }

    return this.claims.some((r => r === claim));
  }

  public setProperties(accesToken: string = null): void {
    if (accesToken) {
      const payload = JSON.parse(window.atob(accesToken.split('.')[1]));

      this.username = payload.name;

      this.claims = Array.isArray(payload.role)
        ? payload.role
        : [payload.role];

      if (payload.tw && Array.isArray(payload.tw)) {
        this.claims.push(...payload.tw);
      }
    } else {
      this.username = UserService.ANONYMOUS;
      this.claims = new Array<string>();
    }

    this.authenticated$.next();
  }
}
