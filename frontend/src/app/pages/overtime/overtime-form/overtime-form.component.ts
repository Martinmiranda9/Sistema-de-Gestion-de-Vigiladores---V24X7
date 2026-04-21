import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DatePickerModule } from 'primeng/datepicker';
import { ToastModule } from 'primeng/toast';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { DividerModule } from 'primeng/divider';
import { MessageService } from 'primeng/api';

import { PayrollConfigService, PayrollConfigCreate, PayrollConfig } from '../../../core/services/payroll-config.service';

@Component({
  selector: 'app-overtime-form',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    ButtonModule, InputTextModule, InputNumberModule, DatePickerModule,
    ToastModule, BreadcrumbModule, DividerModule
  ],
  providers: [MessageService],
  templateUrl: './overtime-form.component.html',
  styleUrl: './overtime-form.component.css'
})
export class OvertimeFormComponent implements OnInit {
  mode: 'actualizar' | 'programar' = 'actualizar';

  breadcrumbItems: any[] = [];
  breadcrumbHome = { icon: 'pi pi-home', routerLink: '/dashboard' };

  pageTitle = '';
  pageSubtitle = '';
  headerIcon = '';

  formData = {
    rate: null as number | null,
    reason: '',
    changedBy: '',
    validFrom: null as Date | null
  };

  currentConfig: PayrollConfig | null = null;
  saving = false;

  // Minimum date for scheduling
  minDate: Date;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private payrollSvc: PayrollConfigService,
    private msgSvc: MessageService
  ) {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.minDate = tomorrow;
  }

  ngOnInit(): void {
    // Determine mode from route path
    const path = this.route.snapshot.url[this.route.snapshot.url.length - 1].path;
    this.mode = path === 'programar' ? 'programar' : 'actualizar';

    this.setupPageText();
    this.loadCurrentConfig();
  }

  setupPageText() {
    if (this.mode === 'actualizar') {
      this.pageTitle = 'Actualizar valor de hora extra';
      this.pageSubtitle = 'Ingresá el nuevo valor vigente a partir de hoy';
      this.headerIcon = 'pi pi-pencil';
      this.formData.validFrom = new Date();
      this.breadcrumbItems = [
        { label: 'Horas Extras', routerLink: '/horas-extras' },
        { label: 'Actualizar Valor' }
      ];
    } else {
      this.pageTitle = 'Programar próximo aumento';
      this.pageSubtitle = 'Programá el aumento de hora extra para una fecha futura';
      this.headerIcon = 'pi pi-calendar-plus';
      this.breadcrumbItems = [
        { label: 'Horas Extras', routerLink: '/horas-extras' },
        { label: 'Programar Aumento' }
      ];
    }
  }

  loadCurrentConfig() {
    this.payrollSvc.getCurrent().subscribe({
      next: (config) => {
        this.currentConfig = config;
        if (this.mode === 'actualizar') {
          // Pre-fill with current value just for reference, though usually left blank to force explicit input
        }
      },
      error: () => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo obtener la configuración actual. Podría no existir.' });
      }
    });
  }

  get isFormValid(): boolean {
    if (!this.formData.rate || this.formData.rate <= 0) return false;
    if (this.mode === 'programar' && !this.formData.validFrom) return false;
    return true;
  }

  cancel() {
    this.router.navigate(['/horas-extras']);
  }

  save() {
    if (!this.isFormValid) return;
    
    // If there is no current config to base off, we assume zeros for other rates since this is only handling ExtraHourRate
    // Ideally the backend should allow partial updates or default to 0. 
    // In this app context we resubmit current normal/night/holiday rates.
    const baseConfig = this.currentConfig || {
      normalHourRate: 0,
      nightSurchargeRate: 0,
      holidayHourRate: 0
    };

    const targetDate = this.mode === 'actualizar' ? new Date() : (this.formData.validFrom as Date);
    
    const dto: PayrollConfigCreate = {
      normalHourRate: baseConfig.normalHourRate,
      nightSurchargeRate: baseConfig.nightSurchargeRate,
      holidayHourRate: baseConfig.holidayHourRate,
      extraHourRate: this.formData.rate as number,
      validFrom: targetDate.toISOString().split('T')[0],
      reason: this.formData.reason.trim() || undefined,
      changedBy: this.formData.changedBy.trim() || undefined
    };

    this.saving = true;
    this.payrollSvc.create(dto).subscribe({
      next: () => {
        this.msgSvc.add({ severity: 'success', summary: 'Éxito', detail: 'La operación se realizó correctamente.' });
        setTimeout(() => {
          this.router.navigate(['/horas-extras']);
        }, 1500);
      },
      error: () => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'Ocurrió un error al guardar.' });
        this.saving = false;
      }
    });
  }
}
