import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { timeout, retry } from 'rxjs/operators';

export interface PayrollConfig {
  id: number;
  normalHourRate: number;
  nightSurchargeRate: number;
  holidayHourRate: number;
  extraHourRate: number;
  validFrom: string;
  reason?: string;
  changedBy?: string;
  createdAt: string;
}

export interface PayrollConfigCreate {
  normalHourRate: number;
  nightSurchargeRate: number;
  holidayHourRate: number;
  extraHourRate: number;
  validFrom: string;
  reason?: string;
  changedBy?: string;
}

@Injectable({ providedIn: 'root' })
export class PayrollConfigService {
  private apiUrl = `${environment.apiUrl}/PayrollConfigs`;
  private readonly TIMEOUT_MS = 10_000;

  constructor(private http: HttpClient) {}

  getAll(): Observable<PayrollConfig[]> {
    return this.http.get<PayrollConfig[]>(this.apiUrl).pipe(
      timeout(this.TIMEOUT_MS),
      retry(1)
    );
  }

  getCurrent(date: Date = new Date()): Observable<PayrollConfig> {
    const iso = date.toISOString().split('T')[0];
    return this.http.get<PayrollConfig>(`${this.apiUrl}/current?date=${iso}`).pipe(
      timeout(this.TIMEOUT_MS)
    );
  }

  create(dto: PayrollConfigCreate): Observable<PayrollConfig> {
    return this.http.post<PayrollConfig>(this.apiUrl, dto);
  }

  update(id: number, dto: PayrollConfigCreate): Observable<PayrollConfig> {
    return this.http.put<PayrollConfig>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
