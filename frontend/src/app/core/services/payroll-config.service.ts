import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, map } from 'rxjs';

export interface PayrollConfig {
  id: number;
  normalHourRate: number;
  nightSurchargeRate: number;
  holidayHourRate: number;
  extraHourRate: number;
  validFrom: string;
}

@Injectable({
  providedIn: 'root'
})
export class PayrollConfigService {
  private apiUrl = `${environment.apiUrl}/PayrollConfigs`;

  constructor(private http: HttpClient) {}

  getLatestConfig(): Observable<PayrollConfig | null> {
    return this.http.get<PayrollConfig[]>(this.apiUrl).pipe(
      map(configs => {
        if (!configs || configs.length === 0) return null;
        // RxJS map encapsulates the business logic of retrieving the latest valid configuration
        return configs.sort((a, b) => new Date(b.validFrom).getTime() - new Date(a.validFrom).getTime())[0];
      })
    );
  }
}
