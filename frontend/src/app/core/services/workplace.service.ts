import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Workplace, WorkplaceCreate, WorkplaceUpdate } from '../models';

@Injectable({
  providedIn: 'root'
})
export class WorkplaceService {
  private apiUrl = `${environment.apiUrl}/Workplaces`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Workplace[]> {
    return this.http.get<Workplace[]>(this.apiUrl);
  }

  getById(id: number): Observable<Workplace> {
    return this.http.get<Workplace>(`${this.apiUrl}/${id}`);
  }

  create(workplace: WorkplaceCreate): Observable<Workplace> {
    return this.http.post<Workplace>(this.apiUrl, workplace);
  }

  update(id: number, workplace: WorkplaceUpdate): Observable<Workplace> {
    return this.http.put<Workplace>(`${this.apiUrl}/${id}`, workplace);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
