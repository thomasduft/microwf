import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { ApiService } from '../shared/services/api.service';
import { HttpWrapperService } from '../shared/services/httpWrapper.service';

import {
  Workflow,
  WorkflowDefinition,
  WorkflowHistory,
  WorkflowVariable
} from './models';
import { WorkflowPagingModel } from './models';

@Injectable({
  providedIn: 'root'
})
export class WorkflowService {
  public constructor(
    private _api: ApiService,
    private _http: HttpWrapperService,
    private _client: HttpClient
  ) { }

  public workflows(page: WorkflowPagingModel): Observable<any> {
    const url = this._api.createApiUrl('workflow');

    let params = new HttpParams()
      .set('pageIndex', page.pageIndex.toString())
      .set('pageSize', page.pageSize.toString());

    if (page.type) {
      params = params.append('type', page.type);
    }
    if (page.correlationId) {
      params = params.append('correlationId', page.correlationId.toString());
    }
    if (page.assignee) {
      params = params.append('assignee', page.assignee);
    }

    return this._client.get<Array<Workflow>>(url, {
      params: params,
      observe: 'response'
    });
  }

  public get(id: number): Observable<Workflow> {
    return this._http.get<Workflow>(`workflow/${id}`);
  }

  public getHistory(id: number): Observable<Array<WorkflowHistory>> {
    return this._http.get<Array<WorkflowHistory>>(`workflow/${id}/history`);
  }

  public getVariables(id: number): Observable<Array<WorkflowVariable>> {
    return this._http.get<Array<WorkflowVariable>>(`workflow/${id}/variables`);
  }

  public definitions(): Observable<Array<WorkflowDefinition>> {
    return this._http.get<Array<WorkflowDefinition>>('workflow/definitions');
  }

  public dot(type: string): Observable<string> {
    return this._http.getAsText(`workflow/dot/${type}`);
  }

  public dotWithHistory(type: string, correlationId: number): Observable<string> {
    return this._http.getAsText(`workflow/dotwithhistory/${type}/${correlationId}`);
  }
}
