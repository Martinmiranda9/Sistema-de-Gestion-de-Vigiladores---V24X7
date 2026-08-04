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

import { PayrollConfigService } from '../../../core/services/payroll-config.service';
import { PayrollConfig, PayrollConfigCreate } from '../../../core/models';

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
    // Hora extra (siempre visible)
    rate: null as number | null,
    // Campos adicionales visibles solo en el modo "configuración inicial"
    normalHourRate: null as number | null,
    nightSurchargeRate: 0 as number,
    holidayHourRate: null as number | null,
    // Auditoría
    reason: '',
    changedBy: '',
    validFrom: null as Date | null
  };

  currentConfig: PayrollConfig | null = null;
  /** true mientras se está cargando la config actual */
  loadingConfig = true;
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
    this.loadingConfig = true;
    this.payrollSvc.getCurrent().subscribe({
      next: (config) => {
        this.currentConfig = config;
        this.loadingConfig = false;
        // Pre-rellenar todos los campos con los valores vigentes
        this.formData.normalHourRate   = config.normalHourRate;
        this.formData.nightSurchargeRate = config.nightSurchargeRate;
        this.formData.holidayHourRate  = config.holidayHourRate;
        this.formData.rate             = config.extraHourRate;
      },
      error: () => {
        // Sin config previa → primer despliegue, mostrar formulario completo
        this.currentConfig = null;
        this.loadingConfig = false;
      }
    });
  }

  /** true cuando no hay ninguna configuración previa (primer setup) */
  get isFirstSetup(): boolean {
    return !this.loadingConfig && this.currentConfig === null;
  }

  get isFormValid(): boolean {
    // La hora extra siempre es obligatoria
    if (!this.formData.rate || this.formData.rate <= 0) return false;

    // En primer setup también exigimos hora normal y feriado
    if (this.isFirstSetup) {
      if (!this.formData.normalHourRate || this.formData.normalHourRate <= 0) return false;
      if (!this.formData.holidayHourRate || this.formData.holidayHourRate <= 0) return false;
    }

    if (this.mode === 'programar' && !this.formData.validFrom) return false;
    return true;
  }

  cancel() {
    this.router.navigate(['/horas-extras']);
  }

  save() {
    if (!this.isFormValid) return;

    // Usar los valores del formulario siempre (no depender de currentConfig)
    const dto: PayrollConfigCreate = {
      normalHourRate:      this.formData.normalHourRate   ?? this.currentConfig!.normalHourRate,
      nightSurchargeRate:  this.formData.nightSurchargeRate,
      holidayHourRate:     this.formData.holidayHourRate  ?? this.currentConfig!.holidayHourRate,
      extraHourRate:       this.formData.rate as number,
      validFrom: this.mode === 'actualizar'
        ? new Date().toISOString().split('T')[0]
        : (this.formData.validFrom as Date).toISOString().split('T')[0],
      reason:     this.formData.reason.trim()     || undefined,
      changedBy:  this.formData.changedBy.trim()  || undefined
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
