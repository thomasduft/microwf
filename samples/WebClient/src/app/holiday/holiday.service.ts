import { Observable } from 'rxjs';

import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { ApiService } from '../shared/services/api.service';

import { Holiday } from './models';
import { HolidayModule } from './holiday.module';

@Injectable({
  providedIn: HolidayModule
})
export class HolidayService {
  public constructor(
    private _http: HttpClient,
    private _api: ApiService
  ) { }

  public contacts(): Observable<Array<Holiday>> {
    const url = this._api.createApiUrl('holiday/mywork');

    return this._http.get<Array<Holiday>>(url);
  }

  public get(id: string): Observable<Holiday> {
    const url = this._api.createApiUrl(`holiday/${id}`);

    return this._http.get<Holiday>(url);
  }

  public process(trigger: string, model: Holiday): Observable<any> {
    // new, apply approve, reject
    const url = this._api.createApiUrl(`holiday/${trigger}`);

    return this._http.post<any>(url, model);
  }
}