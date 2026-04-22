import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  OvertimeSpreadsheetCreatePayload,
  OvertimeSpreadsheetDetail,
  OvertimeSpreadsheetSummary
} from '../models';

@Injectable({ providedIn: 'root' })
export class OvertimeSpreadsheetService {
  private apiUrl = `${environment.apiUrl}/OvertimeSpreadsheets`;

  constructor(private http: HttpClient) {}

  create(payload: OvertimeSpreadsheetCreatePayload): Observable<OvertimeSpreadsheetDetail> {
    return this.http.post<OvertimeSpreadsheetDetail>(this.apiUrl, payload);
  }

  getById(id: number): Observable<OvertimeSpreadsheetDetail> {
    return this.http.get<OvertimeSpreadsheetDetail>(`${this.apiUrl}/${id}`);
  }

  getHistory(month?: number | null, year?: number | null, search?: string): Observable<OvertimeSpreadsheetSummary[]> {
    let params = new HttpParams();

    if (month != null) {
      params = params.set('month', String(month));
    }
    if (year != null) {
      params = params.set('year', String(year));
    }
    if (search?.trim()) {
      params = params.set('search', search.trim());
    }

    return this.http.get<OvertimeSpreadsheetSummary[]>(this.apiUrl, { params });
  }
}
