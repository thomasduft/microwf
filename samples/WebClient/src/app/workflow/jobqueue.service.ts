import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { ApiService } from '../shared/services/api.service';
import { HttpWrapperService } from '../shared/services/httpWrapper.service';

import { PagingModel } from '../shared/services/models';
import { QueueItem } from './models';

@Injectable({
  providedIn: 'root'
})
export class JobQueueService {
  public constructor(
    private _api: ApiService,
    private _http: HttpWrapperService,
    private _client: HttpClient
  ) { }

  public snapshot(): Observable<Array<QueueItem>> {
    return this._http.get<Array<QueueItem>>('jobqueue/snapshot');
  }

  public upcommings(page: PagingModel): Observable<any> {
    const url = this._api.createApiUrl('jobqueue/upcommings');

    const params = new HttpParams()
      .set('pageIndex', page.pageIndex.toString())
      .set('pageSize', page.pageSize.toString());

    return this._client.get<Array<QueueItem>>(url, {
      params: params,
      observe: 'response'
    });
  }

  public failed(page: PagingModel): Observable<any> {
    const url = this._api.createApiUrl('jobqueue/failed');

    const params = new HttpParams()
      .set('pageIndex', page.pageIndex.toString())
      .set('pageSize', page.pageSize.toString());

    return this._client.get<Array<QueueItem>>(url, {
      params: params,
      observe: 'response'
    });
  }
}
