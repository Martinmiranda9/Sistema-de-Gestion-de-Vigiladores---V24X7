import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';

import { MessageService } from 'primeng/api';

import { WorkplaceService } from '../../core/services/workplace.service';
import { SecurityGuardService } from '../../core/services/security-guard.service';
import { PayrollConfigService } from '../../core/services/payroll-config.service';
import { OvertimeSpreadsheetService } from '../../core/services/overtime-spreadsheet.service';
import { OvertimeSpreadsheetCreatePayload, PayrollConfig, SecurityGuard, SpreadsheetRow, Workplace } from '../../core/models';

@Component({
  selector: 'app-overtime-spreadsheet',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    InputNumberModule, ButtonModule,
    CheckboxModule, ToastModule, TooltipModule
  ],
  providers: [MessageService],
  templateUrl: './overtime-spreadsheet.component.html',
  styleUrl: './overtime-spreadsheet.component.css'
})
export class OvertimeSpreadsheetComponent implements OnInit {

  // ── Dropdowns ─────────────────────────────────────────────────────────────
  workplaces: Workplace[] = [];
  selectedWorkplaceId: number | null = null;
  loadingWorkplaces = true;

  // ── Payroll config ────────────────────────────────────────────────────────
  extraHourRate = 0;
  rateValidFrom = '';
  loadingRate = true;

  // ── Month selector ────────────────────────────────────────────────────────
  months = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
  ];
  selectedMonth: number;
  selectedYear: number;

  // ── Spreadsheet ───────────────────────────────────────────────────────────
  rows: SpreadsheetRow[] = [];
  loadingGuards = false;
  saving = false;

  // ── All guards (for manual add) ───────────────────────────────────────────
  allGuards: SecurityGuard[] = [];

  constructor(
    private workplaceSvc: WorkplaceService,
    private guardSvc: SecurityGuardService,
    private payrollSvc: PayrollConfigService,
    private overtimeSpreadsheetSvc: OvertimeSpreadsheetService,
    private msgSvc: MessageService
  ) {
    const now = new Date();
    this.selectedMonth = now.getMonth();
    this.selectedYear = now.getFullYear();
  }

  ngOnInit(): void {
    this.loadWorkplaces();
    this.loadRate();
  }

  // ── Computed ──────────────────────────────────────────────────────────────
  get selectedWorkplace(): Workplace | null {
    return this.workplaces.find(w => w.id === this.selectedWorkplaceId) ?? null;
  }

  // ── Data loading ──────────────────────────────────────────────────────────
  loadWorkplaces(): void {
    this.loadingWorkplaces = true;
    this.workplaceSvc.getAll().subscribe({
      next: (data) => {
        this.workplaces = data.filter(w => w.isActive);
        this.loadingWorkplaces = false;
      },
      error: () => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los objetivos.' });
        this.loadingWorkplaces = false;
      }
    });
  }

  loadRate(): void {
    this.loadingRate = true;

    // La card muestra siempre el valor vigente actual, igual que dashboard.
    this.payrollSvc.getCurrent().subscribe({
      next: (cfg: PayrollConfig) => {
        this.extraHourRate = cfg.extraHourRate;
        this.rateValidFrom = cfg.validFrom;
        this.loadingRate = false;
        this.recalculateAll();
      },
      error: () => {
        // No hay configuración válida para ese mes
        this.extraHourRate = 0;
        this.rateValidFrom = '';
        this.loadingRate = false;
        this.recalculateAll();
        this.msgSvc.add({ severity: 'error', summary: 'Sin valor', detail: 'No hay un valor de hora extra configurado para este período.' });
      }
    });
  }

  onPeriodChange(): void {
    this.loadRate();
  }

  private recalculateAll(): void {
    this.rows.forEach(r => this.onHoursChange(r));
  }

  // ── Workplace selection ───────────────────────────────────────────────────
  onWorkplaceChange(): void {
    if (!this.selectedWorkplaceId) {
      this.rows = [];
      return;
    }
    this.loadingGuards = true;
    this.guardSvc.getByWorkplace(this.selectedWorkplaceId).subscribe({
      next: (guards) => {
        this.rows = guards
          .filter(g => g.isActive)
          .map(g => ({
            guardId: g.id,
            fullName: g.fullName || `${g.lastName}, ${g.firstName}`,
            dni: g.dni,
            fileNumber: g.fileNumber || '—',
            hours: 0,
            total: 0,
            verified: false,
            autoFilled: false // Phase 2: will be true when data comes from shift records
          }));
        this.loadingGuards = false;
      },
      error: () => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los vigiladores del objetivo.' });
        this.loadingGuards = false;
      }
    });
  }

  // ── Reactive calculations ─────────────────────────────────────────────────
  onHoursChange(row: SpreadsheetRow): void {
    row.total = (row.hours || 0) * this.extraHourRate;
  }

  get grandTotal(): number {
    return this.rows.reduce((sum, r) => sum + r.total, 0);
  }

  get totalHours(): number {
    return this.rows.reduce((sum, r) => sum + (r.hours || 0), 0);
  }

  get verifiedCount(): number {
    return this.rows.filter(r => r.verified).length;
  }

  // ── Manual add guard ──────────────────────────────────────────────────────
  addGuardManually(): void {
    if (this.allGuards.length === 0) {
      this.guardSvc.getAll().subscribe({
        next: (guards) => {
          this.allGuards = guards.filter(g => g.isActive);
          this.showAddRow();
        },
        error: () => this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los vigiladores.' })
      });
    } else {
      this.showAddRow();
    }
  }

  private showAddRow(): void {
    this.rows.push({
      guardId: 0,
      fullName: '(Nuevo vigilador)',
      dni: '',
      fileNumber: '',
      hours: 0,
      total: 0,
      verified: false,
      autoFilled: false
    });
  }

  removeRow(index: number): void {
    this.rows.splice(index, 1);
  }

  saveSpreadsheet(): void {
    if (!this.selectedWorkplaceId) {
      this.msgSvc.add({ severity: 'warn', summary: 'Atención', detail: 'Seleccioná un objetivo antes de guardar.' });
      return;
    }

    if (this.rows.length === 0) {
      this.msgSvc.add({ severity: 'warn', summary: 'Atención', detail: 'No hay datos para guardar.' });
      return;
    }

    const rowsToSave = this.rows
      .filter(r => (r.hours || 0) > 0 || (r.total || 0) > 0)
      .map(r => ({
        securityGuardId: r.guardId > 0 ? r.guardId : null,
        fullName: r.fullName,
        dni: r.dni,
        fileNumber: r.fileNumber,
        hours: r.hours || 0,
        total: r.total || 0,
        verified: r.verified
      }));

    if (rowsToSave.length === 0) {
      this.msgSvc.add({ severity: 'warn', summary: 'Atención', detail: 'Ingresá al menos una fila con horas para guardar.' });
      return;
    }

    const payload: OvertimeSpreadsheetCreatePayload = {
      workplaceId: this.selectedWorkplaceId,
      month: this.selectedMonth + 1,
      year: this.selectedYear,
      extraHourRate: this.extraHourRate,
      rateValidFrom: this.rateValidFrom || null,
      rows: rowsToSave
    };

    this.saving = true;
    this.overtimeSpreadsheetSvc.create(payload).subscribe({
      next: (saved) => {
        this.saving = false;
        this.msgSvc.add({
          severity: 'success',
          summary: 'Guardado',
          detail: `Planilla guardada correctamente (#${saved.id}).`
        });
      },
      error: () => {
        this.saving = false;
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo guardar la planilla.' });
      }
    });
  }

  // ── Print ─────────────────────────────────────────────────────────────────
  printSpreadsheet(): void {
    window.print();
  }

  // ── Helpers ───────────────────────────────────────────────────────────────
  formatCurrency(value: number): string {
    return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS', maximumFractionDigits: 2 }).format(value);
  }

  get monthLabel(): string {
    return `${this.months[this.selectedMonth]} ${this.selectedYear}`;
  }
}
