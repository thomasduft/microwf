import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { ApiService } from '../shared/services/api.service';
import { WorkflowResult, AssigneeWorkflowResult } from '../workflow/index';

import { Issue, IssueViewmodel } from './models';

@Injectable()
export class IssueService {
  public constructor(
    private _http: HttpClient,
    private _api: ApiService
  ) { }

  public myWork(): Observable<Array<Issue>> {
    const url = this._api.createApiUrl('issue/mywork');

    return this._http.get<Array<Issue>>(url);
  }

  public create(): Observable<WorkflowResult<Issue, IssueViewmodel>> {
    const url = this._api.createApiUrl(`issue/new`);

    return this._http.post<WorkflowResult<Issue, IssueViewmodel>>(url, null);
  }

  public save(model: IssueViewmodel): Observable<number> {
    const url = this._api.createApiUrl(`issue`);

    return this._http.post<number>(url, model);
  }

  public get(id: string): Observable<WorkflowResult<null, Issue>> {
    const url = this._api.createApiUrl(`issue/${id}`);

    return this._http.get<WorkflowResult<null, Issue>>(url);
  }

  public process(model: IssueViewmodel): Observable<WorkflowResult<Issue, AssigneeWorkflowResult>> {
    const url = this._api.createApiUrl(`issue/process`);

    return this._http.post<WorkflowResult<Issue, AssigneeWorkflowResult>>(url, model);
  }
}
