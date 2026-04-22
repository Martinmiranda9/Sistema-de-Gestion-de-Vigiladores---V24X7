import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface OvertimeSpreadsheetRowPayload {
  securityGuardId: number | null;
  fullName: string;
  dni: string;
  fileNumber: string;
  hours: number;
  total: number;
  verified: boolean;
}

export interface OvertimeSpreadsheetCreatePayload {
  workplaceId: number;
  month: number;
  year: number;
  extraHourRate: number;
  rateValidFrom: string | null;
  rows: OvertimeSpreadsheetRowPayload[];
}

export interface OvertimeSpreadsheetSummary {
  id: number;
  workplaceId: number;
  workplaceName: string;
  month: number;
  year: number;
  extraHourRate: number;
  totalHours: number;
  grandTotal: number;
  rowsCount: number;
  verifiedCount: number;
  createdAt: string;
}

export interface OvertimeSpreadsheetRow {
  id: number;
  securityGuardId: number | null;
  fullName: string;
  dni: string;
  fileNumber: string;
  hours: number;
  total: number;
  verified: boolean;
}

export interface OvertimeSpreadsheetDetail {
  id: number;
  workplaceId: number;
  workplaceName: string;
  month: number;
  year: number;
  extraHourRate: number;
  rateValidFrom: string | null;
  totalHours: number;
  grandTotal: number;
  createdAt: string;
  rows: OvertimeSpreadsheetRow[];
}

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
