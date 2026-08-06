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
import { MessageModule } from 'primeng/message';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService } from 'primeng/api';
import { PayrollConfigService } from '../../../core/services/payroll-config.service';
import { RateField } from '../rate-page/rate-page.component';
import { PayrollConfig, PayrollConfigCreate } from '../../../core/models';

@Component({
  selector: 'app-rate-form',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    ButtonModule, InputTextModule, InputNumberModule, DatePickerModule,
    ToastModule, BreadcrumbModule, DividerModule, MessageModule, TooltipModule
  ],
  providers: [MessageService],
  templateUrl: './rate-form.component.html',
  styleUrl: './rate-form.component.css'
})
export class RateFormComponent implements OnInit {
  mode: 'actualizar' | 'programar' = 'actualizar';

  rateField: RateField = 'extraHourRate';
  rateLabel = 'hora extra';
  parentRoute = '/horas-extras';

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
  configLoading = true;
  configError = false;
  saving = false;
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
    const urlSegments = this.route.snapshot.url.map(s => s.path);
    const base = urlSegments.length > 1 ? urlSegments[0] : '';
    const action = urlSegments[urlSegments.length - 1];

    this.mode = action === 'programar' ? 'programar' : 'actualizar';

    if (base === 'hora-nocturna') {
      this.rateField = 'nightSurchargeRate';
      this.rateLabel = 'hora nocturna';
      this.parentRoute = '/hora-nocturna';
    } else if (base === 'hora-feriada') {
      this.rateField = 'holidayHourRate';
      this.rateLabel = 'hora feriada';
      this.parentRoute = '/hora-feriada';
    } else {
      this.rateField = 'extraHourRate';
      this.rateLabel = 'hora extra';
      this.parentRoute = '/horas-extras';
    }

    this.setupPageText();
    this.loadCurrentConfig();
  }

  setupPageText() {
    const labelCap = this.rateLabel.charAt(0).toUpperCase() + this.rateLabel.slice(1);
    if (this.mode === 'actualizar') {
      this.pageTitle = `Actualizar valor de ${this.rateLabel}`;
      this.pageSubtitle = `Ingresá el nuevo valor vigente a partir de hoy`;
      this.headerIcon = 'pi pi-pencil';
      this.formData.validFrom = new Date();
      this.breadcrumbItems = [
        { label: labelCap, routerLink: this.parentRoute },
        { label: 'Actualizar Valor' }
      ];
    } else {
      this.pageTitle = `Programar próximo aumento`;
      this.pageSubtitle = `Programá el aumento de ${this.rateLabel} para una fecha futura`;
      this.headerIcon = 'pi pi-calendar-plus';
      this.breadcrumbItems = [
        { label: labelCap, routerLink: this.parentRoute },
        { label: 'Programar Aumento' }
      ];
    }
  }

  loadCurrentConfig() {
    this.configLoading = true;
    this.configError = false;
    this.payrollSvc.getCurrent().subscribe({
      next: (config) => {
        this.currentConfig = config;
        this.configLoading = false;
      },
      error: (err) => {
        this.configLoading = false;
        // 404 = todavía no existe ninguna config (primer uso del sistema) → se permite guardar
        // Cualquier otro error (500, timeout) → bloqueamos para evitar enviar ceros al backend
        if (err?.status === 404) {
          this.currentConfig = null;
        } else {
          this.configError = true;
          this.msgSvc.add({
            severity: 'error',
            summary: 'Error de conexión',
            detail: 'No se pudo obtener la configuración actual. Reintentá en unos segundos.'
          });
        }
      }
    });
  }

  get isFormValid(): boolean {
    if (this.configLoading || this.configError) return false;
    if (!this.formData.rate || this.formData.rate <= 0) return false;
    if (this.mode === 'programar' && !this.formData.validFrom) return false;
    return true;
  }

  cancel() {
    this.router.navigate([this.parentRoute]);
  }

  save() {
    if (!this.isFormValid) return;

    // Si no hay config previa (primer uso del sistema), los rates no editados
    // usan el mínimo válido (1) para no romper la validación del backend.
    // El usuario puede actualizarlos individualmente desde cada sección.
    const baseConfig = this.currentConfig ?? {
      normalHourRate: 1,
      nightSurchargeRate: 0,
      holidayHourRate: 1,
      extraHourRate: 1
    };

    const targetDate = this.mode === 'actualizar' ? new Date() : (this.formData.validFrom as Date);

    const dto: PayrollConfigCreate = {
      normalHourRate: baseConfig.normalHourRate,
      nightSurchargeRate: baseConfig.nightSurchargeRate,
      holidayHourRate: baseConfig.holidayHourRate,
      extraHourRate: baseConfig.extraHourRate,
      validFrom: targetDate.toISOString().split('T')[0],
      reason: this.formData.reason.trim() || undefined,
      changedBy: this.formData.changedBy.trim() || undefined
    };

    (dto as any)[this.rateField] = this.formData.rate as number;

    this.saving = true;
    this.payrollSvc.create(dto).subscribe({
      next: () => {
        this.msgSvc.add({ severity: 'success', summary: 'Éxito', detail: 'La operación se realizó correctamente.' });
        setTimeout(() => this.router.navigate([this.parentRoute]), 1500);
      },
      error: () => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'Ocurrió un error al guardar.' });
        this.saving = false;
      }
    });
  }
}
