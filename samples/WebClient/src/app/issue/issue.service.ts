import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';

import { HttpWrapperService } from '../shared/services/httpWrapper.service';
import { WorkflowResult, AssigneeWorkflowResult } from '../workflow/index';

import { Issue, IssueViewmodel } from './models';

@Injectable()
export class IssueService {
  public constructor(
    private _http: HttpWrapperService
  ) { }

  public assignees(): Observable<Array<string>> {
    return this._http.get<Array<string>>('issue/assignees');
  }

  public myWork(): Observable<Array<Issue>> {
    return this._http.get<Array<Issue>>('issue/mywork');
  }

  public create(): Observable<WorkflowResult<Issue, IssueViewmodel>> {
    return this._http.post<WorkflowResult<Issue, IssueViewmodel>>(`issue/new`, null);
  }

  public save(model: IssueViewmodel): Observable<number> {
    return this._http.post<number>(`issue`, model);
  }

  public get(id: string): Observable<WorkflowResult<null, Issue>> {
    return this._http.get<WorkflowResult<null, Issue>>(`issue/${id}`);
  }

  public process(
    model: IssueViewmodel
  ): Observable<WorkflowResult<Issue, AssigneeWorkflowResult>> {
    return this._http
      .post<WorkflowResult<Issue, AssigneeWorkflowResult>>(`issue/process`, model);
  }
}
