import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { ApiService } from '../shared/services/api.service';

import { WorkflowDefinition } from './models';

@Injectable({
  providedIn: 'root'
})
export class WorkflowService {
  public constructor(
    private _http: HttpClient,
    private _api: ApiService
  ) { }

  public definitions(): Observable<Array<WorkflowDefinition>> {
    const url = this._api.createApiUrl('workflow/definitions');

    return this._http.get<Array<WorkflowDefinition>>(url);
  }

  public dot(type: string): Observable<string> {
    const url = this._api.createApiUrl(`workflow/dot/${type}`);

    return this._http.get(url, { responseType: 'text' });
  }
}
