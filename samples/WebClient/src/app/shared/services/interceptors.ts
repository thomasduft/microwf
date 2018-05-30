import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

import {
  Injectable,
  EventEmitter
} from '@angular/core';
import {
  HttpClient,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpEvent,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams,
  HttpResponse
} from '@angular/common/http';

import { ServicesModule } from './services.module';

import { AuthService } from './auth.service';
import { MessageBus } from './messageBus.service';

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
    private _messageBus: MessageBus
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
            // redirect to the login route
            // or show a modal
          }
        }
      }));
  }
}
