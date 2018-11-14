import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';

import { HttpWrapperService } from '../shared/services/httpWrapper.service';
import { WorkflowResult, AssigneeWorkflowResult } from '../workflow/index';

import { Holiday, ApplyHoliday, ApproveHoliday } from './models';

@Injectable()
export class HolidayService {
  public constructor(
    private _http: HttpWrapperService
  ) { }

  public myWork(): Observable<Array<Holiday>> {
    return this._http.get<Array<Holiday>>('holiday/mywork');
  }

  public get(id: string): Observable<WorkflowResult<null, Holiday>> {
    return this._http.get<WorkflowResult<null, Holiday>>(`holiday/${id}`);
  }

  public create(): Observable<WorkflowResult<Holiday, ApplyHoliday>> {
    return this._http.post<WorkflowResult<Holiday, ApplyHoliday>>(`holiday/new`, null);
  }

  public apply(
    model: ApplyHoliday
  ): Observable<WorkflowResult<Holiday, AssigneeWorkflowResult>> {
    return this._http
      .post<WorkflowResult<Holiday, AssigneeWorkflowResult>>(`holiday/apply`, model);
  }

  public approve(
    model: ApproveHoliday
  ): Observable<WorkflowResult<Holiday, AssigneeWorkflowResult>> {
    return this._http
      .post<WorkflowResult<Holiday, AssigneeWorkflowResult>>(`holiday/approve`, model);
  }

  public reject(
    model: ApproveHoliday
  ): Observable<WorkflowResult<Holiday, AssigneeWorkflowResult>> {
    return this._http
      .post<WorkflowResult<Holiday, AssigneeWorkflowResult>>(`holiday/reject`, model);
  }
}
