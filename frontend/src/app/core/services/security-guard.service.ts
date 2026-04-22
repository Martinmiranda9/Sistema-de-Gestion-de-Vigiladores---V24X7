import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, map } from 'rxjs';
import { SecurityGuard, SecurityGuardCreate, SecurityGuardUpdate } from '../models';

@Injectable({
  providedIn: 'root'
})
export class SecurityGuardService {
  private apiUrl = `${environment.apiUrl}/SecurityGuards`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<SecurityGuard[]> {
    return this.http.get<SecurityGuard[]>(this.apiUrl);
  }

  getById(id: number): Observable<SecurityGuard> {
    return this.http.get<SecurityGuard>(`${this.apiUrl}/${id}`);
  }

  create(guard: SecurityGuardCreate): Observable<SecurityGuard> {
    return this.http.post<SecurityGuard>(this.apiUrl, guard);
  }

  update(id: number, guard: SecurityGuardUpdate): Observable<SecurityGuard> {
    return this.http.put<SecurityGuard>(`${this.apiUrl}/${id}`, guard);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getByWorkplace(workplaceId: number): Observable<SecurityGuard[]> {
    return this.http.get<SecurityGuard[]>(`${this.apiUrl}/by-workplace/${workplaceId}`);
  }

  getActiveGuardsCount(): Observable<number> {
    return this.getAll().pipe(
      map(guards => guards.length)
    );
  }
}
