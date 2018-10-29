import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';

import { HttpWrapperService } from '../shared/services/httpWrapper.service';

import {
  Workflow,
  WorkflowDefinition,
  WorkflowHistory,
  WorkflowVariable
} from './models';
import { PagingModel } from '../shared/services/models';

@Injectable({
  providedIn: 'root'
})
export class WorkflowService {
  public constructor(
    private _http: HttpWrapperService,
  ) { }

  public workflows(page: PagingModel): Observable<Array<Workflow>> {
    return this._http.getList<Array<Workflow>>('workflow', page);
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

  public getInstance(type: string, correlationId: number): Observable<Workflow> {
    const params = new HttpParams()
      .set('type', type)
      .set('correlationId', correlationId.toString());

    return this._http.get<Workflow>(`workflow/instance`, params);
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
