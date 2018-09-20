import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { ApiService } from '../shared/services/api.service';

import { Workflow, WorkflowDefinition } from './models';

@Injectable({
  providedIn: 'root'
})
export class WorkflowService {
  public constructor(
    private _http: HttpClient,
    private _api: ApiService
  ) { }

  public workflows(): Observable<Array<Workflow>> {
    const url = this._api.createApiUrl('workflow');

    return this._http.get<Array<Workflow>>(url);
  }

  public get(id: number): Observable<Workflow> {
    const url = this._api.createApiUrl(`workflow/${id}`);

    return this._http.get<Workflow>(url);
  }

  // TODO: workflow/{id}/history
  // TODO: workflow/{id}/variables

  public mywork(): Observable<Array<Workflow>> {
    const url = this._api.createApiUrl('workflow/mywork');

    return this._http.get<Array<Workflow>>(url);
  }

  public getInstance(type: string, correlationId: number): Observable<Workflow> {
    const url = this._api.createApiUrl(`workflow/instance`);
    const params = new HttpParams()
      .set('type', type)
      .set('correlationId', correlationId.toString());

    return this._http.get<Workflow>(url, { params });
  }

  public definitions(): Observable<Array<WorkflowDefinition>> {
    const url = this._api.createApiUrl('workflow/definitions');

    return this._http.get<Array<WorkflowDefinition>>(url);
  }

  public dot(type: string): Observable<string> {
    const url = this._api.createApiUrl(`workflow/dot/${type}`);

    return this._http.get(url, { responseType: 'text' });
  }
}
