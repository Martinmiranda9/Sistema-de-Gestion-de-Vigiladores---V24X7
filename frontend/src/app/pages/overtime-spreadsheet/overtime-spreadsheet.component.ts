import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';

import { MessageService } from 'primeng/api';

import { WorkplaceService, Workplace } from '../../core/services/workplace.service';
import { SecurityGuardService, SecurityGuard } from '../../core/services/security-guard.service';
import { PayrollConfigService, PayrollConfig } from '../../core/services/payroll-config.service';

/** Row in the spreadsheet */
export interface SpreadsheetRow {
  guardId: number;
  fullName: string;
  dni: string;
  fileNumber: string;
  hours: number;
  total: number;
  verified: boolean;
  /** Phase 2: flag indicating data came from shift records */
  autoFilled: boolean;
}

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
    // Buscamos el valor vigente al ÚLTIMO día del mes seleccionado
    // new Date(year, month + 1, 0) da el último día del mes actual.
    // Usamos 23:59:59 para asegurarnos de que cubra cualquier creación de ese día.
    const targetDate = new Date(this.selectedYear, this.selectedMonth + 1, 0, 23, 59, 59);
    
    this.payrollSvc.getCurrent(targetDate).subscribe({
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

  // ── Save (placeholder — Phase 2) ──────────────────────────────────────────
  saveSpreadsheet(): void {
    if (this.rows.length === 0) {
      this.msgSvc.add({ severity: 'warn', summary: 'Atención', detail: 'No hay datos para guardar.' });
      return;
    }
    this.saving = true;
    // TODO Phase 2: POST to backend endpoint
    setTimeout(() => {
      this.saving = false;
      this.msgSvc.add({ severity: 'success', summary: 'Guardado', detail: 'Planilla guardada correctamente.' });
    }, 800);
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
