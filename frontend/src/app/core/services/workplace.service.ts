import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

export interface Workplace {
  id: number;
  name: string;
  address: string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class WorkplaceService {
  private apiUrl = `${environment.apiUrl}/Workplaces`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Workplace[]> {
    return this.http.get<Workplace[]>(this.apiUrl);
  }
}
