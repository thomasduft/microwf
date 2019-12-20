import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpEvent, HttpRequest } from '@angular/common/http';

import { PagingModel } from './models';
import { ApiService } from './api.service';

import { ServicesModule } from './services.module';

@Injectable({
  providedIn: ServicesModule
})
export class HttpWrapperService {
  public constructor(
    private http: HttpClient,
    private _api: ApiService
  ) { }

  // Sample usage...!
  // getAllCustomers() {
  //   this.customerDataService.getAll<Customer[]>()
  //     .subscribe((result: any) => {
  //       this.totalCount = JSON.parse(result.headers.get('X-Pagination')).totalCount;
  //       this.dataSource = result.body.value;
  //     });
  // }

  public getList<T>(endpoint: string, pagingModel: PagingModel): Observable<any> {
    const url = this._api.createApiUrl(endpoint);

    const params = new HttpParams()
      .set('pageIndex', pagingModel.pageIndex.toString())
      .set('pageSize', pagingModel.pageSize.toString());

    return this.http.get<T>(url, {
      params: params,
      observe: 'response'
    });
  }

  public get<T>(endpoint: string, params?: HttpParams): Observable<T> {
    const url = this._api.createApiUrl(endpoint);

    return this.http.get<T>(url, { params });
  }

  public getAsText<T>(endpoint: string): Observable<string> {
    const url = this._api.createApiUrl(endpoint);

    return this.http.get(url, { responseType: 'text' });
  }

  public getBlob(endpoint: string): Observable<Blob> {
    const url = this._api.createApiUrl(endpoint);

    return this.http.get(url, { responseType: 'blob' });
  }

  public post<T>(endpoint: string, body: any): Observable<T> {
    const url = this._api.createApiUrl(endpoint);

    return this.http.post<T>(url, body);
  }

  public postFile(
    endpoint: string,
    file: File,
    useProgress: boolean = false
  ): Observable<HttpEvent<unknown>> {
    const url = this._api.createApiUrl(endpoint);
    const formData: FormData = new FormData();
    formData.append('file', file);

    const req = new HttpRequest('POST', url, formData, {
      reportProgress: useProgress
    });

    return this.http.request(req);
  }

  public put<T>(endpoint: string, body: string): Observable<T> {
    const url = this._api.createApiUrl(endpoint);

    return this.http.put<T>(url, body);
  }

  public delete<T>(endpoint: string): Observable<T> {
    const url = this._api.createApiUrl(endpoint);

    return this.http.delete<T>(url);
  }

  public patch<T>(endpoint: string, body: string): Observable<T> {
    const url = this._api.createApiUrl(endpoint);

    return this.http.patch<T>(url, body);
  }
}
