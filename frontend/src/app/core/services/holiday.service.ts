import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { timeout, retry } from 'rxjs/operators';
import { Holiday, HolidayCreate } from '../models';

@Injectable({ providedIn: 'root' })
export class HolidayService {
  private apiUrl = `${environment.apiUrl}/Holidays`;
  private readonly TIMEOUT_MS = 10_000;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Holiday[]> {
    return this.http.get<Holiday[]>(this.apiUrl).pipe(
      timeout(this.TIMEOUT_MS), retry(1)
    );
  }

  getByYear(year: number): Observable<Holiday[]> {
    return this.http.get<Holiday[]>(`${this.apiUrl}/year/${year}`).pipe(
      timeout(this.TIMEOUT_MS), retry(1)
    );
  }

  create(dto: HolidayCreate): Observable<Holiday> {
    return this.http.post<Holiday>(this.apiUrl, dto);
  }

  update(id: number, dto: HolidayCreate): Observable<Holiday> {
    return this.http.put<Holiday>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
