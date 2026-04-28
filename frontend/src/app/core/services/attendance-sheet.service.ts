import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AttendanceSheet, AttendanceSheetCreatePayload } from '../models';

@Injectable({
  providedIn: 'root'
})
export class AttendanceSheetService {
  private apiUrl = `${environment.apiUrl}/AttendanceSheets`;

  constructor(private http: HttpClient) { }

  getAll(workplaceId?: number, securityGuardId?: number, month?: number, year?: number): Observable<AttendanceSheet[]> {
    let params = new HttpParams();
    if (workplaceId) params = params.set('workplaceId', workplaceId.toString());
    if (securityGuardId) params = params.set('securityGuardId', securityGuardId.toString());
    if (month) params = params.set('month', month.toString());
    if (year) params = params.set('year', year.toString());

    return this.http.get<AttendanceSheet[]>(this.apiUrl, { params });
  }

  getById(id: number): Observable<AttendanceSheet> {
    return this.http.get<AttendanceSheet>(`${this.apiUrl}/${id}`);
  }

  create(payload: AttendanceSheetCreatePayload): Observable<AttendanceSheet> {
    return this.http.post<AttendanceSheet>(this.apiUrl, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
