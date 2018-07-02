import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { ApiService } from '../shared/services/api.service';
import { WorkflowResult, NoWorkflowResult } from '../workflow/index';

import { Holiday, ApplyHoliday, ApproveHoliday } from './models';

@Injectable()
export class HolidayService {
  public constructor(
    private _http: HttpClient,
    private _api: ApiService
  ) { }

  public myWork(): Observable<Array<Holiday>> {
    const url = this._api.createApiUrl('holiday/mywork');

    return this._http.get<Array<Holiday>>(url);
  }

  public get(id: string): Observable<WorkflowResult<null, Holiday>> {
    const url = this._api.createApiUrl(`holiday/${id}`);

    return this._http.get<WorkflowResult<null, Holiday>>(url);
  }

  public create(): Observable<WorkflowResult<Holiday, ApplyHoliday>> {
    const url = this._api.createApiUrl(`holiday/new`);

    return this._http.post<WorkflowResult<Holiday, ApplyHoliday>>(url, null);
  }

  public apply(model: ApplyHoliday): Observable<WorkflowResult<Holiday, NoWorkflowResult>> {
    const url = this._api.createApiUrl(`holiday/apply`);

    return this._http.post<WorkflowResult<Holiday, NoWorkflowResult>>(url, model);
  }

  public approve(model: ApproveHoliday): Observable<WorkflowResult<Holiday, NoWorkflowResult>> {
    const url = this._api.createApiUrl(`holiday/approve`);

    return this._http.post<WorkflowResult<Holiday, NoWorkflowResult>>(url, model);
  }

  public reject(model: ApproveHoliday): Observable<WorkflowResult<Holiday, NoWorkflowResult>> {
    const url = this._api.createApiUrl(`holiday/reject`);

    return this._http.post<WorkflowResult<Holiday, NoWorkflowResult>>(url, model);
  }
}
