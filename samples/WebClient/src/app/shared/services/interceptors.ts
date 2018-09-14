import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import {
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpEvent,
  HttpErrorResponse,
  HttpResponse,
  HTTP_INTERCEPTORS
} from '@angular/common/http';

import { ServicesModule } from './services.module';

import { AuthService } from './auth.service';

@Injectable({
  providedIn: ServicesModule
})
export class AuthInterceptor implements HttpInterceptor {
  public constructor(
    private _auth: AuthService
  ) { }

  public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this._auth.getToken();
    if (token) {
      req = req.clone({ headers: req.headers.set('Authorization', 'Bearer ' + token) });
    }

    return next.handle(req);
  }
}

@Injectable({
  providedIn: ServicesModule
})
export class HttpErrorInterceptor implements HttpInterceptor {
  public constructor(
    private _router: Router
  ) { }

  public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req)
      .pipe(tap((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          // do stuff with response if you want
        }
      }, (err: any) => {
        if (err instanceof HttpErrorResponse) {
          if (err.status === 401) {
            this._router.navigate(['login']);
          }
          if (err.status === 403) {
            this._router.navigate(['forbidden']);
          }
        }
      }));
  }
}

export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: HttpErrorInterceptor, multi: true }
];
