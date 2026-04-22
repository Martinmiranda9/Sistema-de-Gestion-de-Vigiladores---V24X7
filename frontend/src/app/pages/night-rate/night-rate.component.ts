import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RatePageComponent } from '../../shared/components/rate-page/rate-page.component';
import { PayrollConfigService } from '../../core/services/payroll-config.service';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { PayrollConfig } from '../../core/models';

@Component({
  selector: 'app-night-rate',
  standalone: true,
  imports: [CommonModule, RatePageComponent, ToastModule],
  providers: [MessageService],
  templateUrl: './night-rate.component.html',
  styleUrl: './night-rate.component.css'
})
export class NightRateComponent implements OnInit {
  configs: PayrollConfig[] = [];
  loading = true;

  constructor(
    private payrollSvc: PayrollConfigService,
    private msgSvc: MessageService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.payrollSvc.getAll().subscribe({
      next: (data: PayrollConfig[]) => {
        this.configs = data;
        this.loading = false;
      },
      error: () => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo cargar la configuración.' });
        this.loading = false;
      }
    });
  }

  onDelete(id: number): void {
    this.payrollSvc.delete(id).subscribe({
      next: () => {
        this.msgSvc.add({ severity: 'success', summary: 'Eliminado', detail: 'Registro eliminado.' });
        this.load();
      },
      error: () => this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar.' })
    });
  }
}
