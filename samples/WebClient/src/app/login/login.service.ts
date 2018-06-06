import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';

import { IToken, AuthService } from '../shared/services/auth.service';
import { ApiService } from '../shared/services/api.service';

export class LoginModel {
  public constructor(
    public username: string,
    public password: string
  ) { }
}

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  public constructor(
    private _http: HttpClient,
    private _api: ApiService,
    private _authService: AuthService
  ) { }

  public login(username: string, password: string): Observable<IToken> {
    const url = this._api.createRawUrl('connect/token');

    let body = `grant_type=password`;
    body += `&username=${username}`;
    body += `&password=${password}`;
    body += `&client_id=ro.client`;
    body += `&scope=api1`;

    const options = {
      headers: new HttpHeaders({
        'Content-Type': 'application/x-www-form-urlencoded'
      })
    };

    return this._http.post<IToken>(url, body, options);
  }

  public authenticate(token: IToken): void {
    this._authService.authenticate(token);
  }
}
