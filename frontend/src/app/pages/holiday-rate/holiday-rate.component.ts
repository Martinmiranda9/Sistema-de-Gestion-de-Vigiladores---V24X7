import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RatePageComponent } from '../../shared/components/rate-page/rate-page.component';
import { PayrollConfigService } from '../../core/services/payroll-config.service';
import { HolidayService } from '../../core/services/holiday.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { DialogModule } from 'primeng/dialog';
import { TooltipModule } from 'primeng/tooltip';
import { CheckboxModule } from 'primeng/checkbox';
import { Holiday, HolidayCreate, PayrollConfig } from '../../core/models';

@Component({
  selector: 'app-holiday-rate',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    RatePageComponent, ToastModule, ConfirmDialogModule,
    ButtonModule, InputTextModule, DatePickerModule, DialogModule, TooltipModule,
    CheckboxModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './holiday-rate.component.html',
  styleUrl: './holiday-rate.component.css'
})
export class HolidayRateComponent implements OnInit {
  configs: PayrollConfig[] = [];
  loading = true;

  holidays: Holiday[] = [];
  loadingHolidays = true;
  currentYear: number;

  holidayDialogVisible = false;
  editingHoliday: Holiday | null = null;
  holidayForm: HolidayCreate = {
    date: '',
    description: '',
    isRecurring: false
  };
  holidayDate: Date | null = null;
  savingHoliday = false;

  constructor(
    private payrollSvc: PayrollConfigService,
    private holidaySvc: HolidayService,
    private msgSvc: MessageService,
    private confirmSvc: ConfirmationService
  ) {
    this.currentYear = new Date().getFullYear();
  }

  ngOnInit(): void {
    this.loadConfigs();
    this.loadHolidays();
  }

  loadConfigs(): void {
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

  onDeleteConfig(id: number): void {
    this.payrollSvc.delete(id).subscribe({
      next: () => {
        this.msgSvc.add({ severity: 'success', summary: 'Eliminado', detail: 'Registro eliminado.' });
        this.loadConfigs();
      },
      error: () => this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar.' })
    });
  }

  loadHolidays(): void {
    this.loadingHolidays = true;
    this.holidaySvc.getByYear(this.currentYear).subscribe({
      next: (data: Holiday[]) => {
        this.holidays = data.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
        this.loadingHolidays = false;
      },
      error: () => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los feriados.' });
        this.loadingHolidays = false;
      }
    });
  }

  changeYear(offset: number): void {
    this.currentYear += offset;
    this.loadHolidays();
  }

  openNewHoliday(): void {
    this.editingHoliday = null;
    this.holidayDate = null;
    this.holidayForm = { date: '', description: '', isRecurring: false };
    this.holidayDialogVisible = true;
  }

  openEditHoliday(h: Holiday): void {
    this.editingHoliday = h;
    this.holidayDate = new Date(h.date);
    this.holidayForm = {
      date: h.date,
      description: h.description,
      isRecurring: h.isRecurring
    };
    this.holidayDialogVisible = true;
  }

  saveHoliday(): void {
    if (!this.holidayDate || !this.holidayForm.description.trim()) return;
    this.savingHoliday = true;

    this.holidayForm.date = this.holidayDate.toISOString().split('T')[0];

    const obs = this.editingHoliday
      ? this.holidaySvc.update(this.editingHoliday.id, this.holidayForm)
      : this.holidaySvc.create(this.holidayForm);

    obs.subscribe({
      next: () => {
        this.msgSvc.add({ severity: 'success', summary: 'Éxito', detail: this.editingHoliday ? 'Feriado actualizado.' : 'Feriado agregado.' });
        this.holidayDialogVisible = false;
        this.savingHoliday = false;
        this.loadHolidays();
      },
      error: () => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo guardar el feriado.' });
        this.savingHoliday = false;
      }
    });
  }

  confirmDeleteHoliday(h: Holiday): void {
    this.confirmSvc.confirm({
      message: `¿Estás seguro de que querés eliminar "${h.description}"?`,
      header: 'Confirmar eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, eliminar',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.holidaySvc.delete(h.id).subscribe({
          next: () => {
            this.msgSvc.add({ severity: 'success', summary: 'Eliminado', detail: 'Feriado eliminado.' });
            this.loadHolidays();
          },
          error: () => this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar.' })
        });
      }
    });
  }

  formatHolidayDate(date: string): string {
    const onlyDate = date.split('T')[0];
    return new Intl.DateTimeFormat('es-AR', { day: '2-digit', month: 'long' }).format(new Date(onlyDate + 'T12:00:00'));
  }

  isPast(date: string): boolean {
    const onlyDate = date.split('T')[0];
    return new Date(onlyDate + 'T23:59:59') < new Date();
  }
}
