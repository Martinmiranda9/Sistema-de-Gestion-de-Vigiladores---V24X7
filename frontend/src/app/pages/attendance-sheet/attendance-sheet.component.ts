import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';

import { MessageService } from 'primeng/api';
import { AttendanceDayRow } from '../../core/models';

@Component({
  selector: 'app-attendance-sheet',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    ButtonModule, ToastModule, TooltipModule
  ],
  providers: [MessageService],
  templateUrl: './attendance-sheet.component.html',
  styleUrl: './attendance-sheet.component.css'
})
export class AttendanceSheetComponent {

  // ── Header fields ──────────────────────────────────────────────────────────
  months = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
  ];
  selectedMonth: number;
  selectedYear: number;
  guardName = '';
  workplace = '';

  // ── Table rows ─────────────────────────────────────────────────────────────
  rows: AttendanceDayRow[] = [];

  // ── State ──────────────────────────────────────────────────────────────────
  saving = false;

  constructor(private msgSvc: MessageService) {
    const now = new Date();
    this.selectedMonth = now.getMonth();
    this.selectedYear = now.getFullYear();
    this.buildRows();
  }

  // ── Build/rebuild the 31-day grid ──────────────────────────────────────────
  buildRows(): void {
    this.rows = Array.from({ length: 31 }, (_, i) => ({
      day: i + 1,
      entry: '',
      exit: '',
      isDayOff: false,
      workedHours: 0,
      nightHours: 0,
      notes: ''
    }));
  }

  onPeriodChange(): void {
    this.buildRows();
  }

  // ── Day-off toggle ─────────────────────────────────────────────────────────
  toggleDayOff(row: AttendanceDayRow): void {
    if (row.isDayOff) {
      row.entry = '';
      row.exit = '';
      row.workedHours = 0;
      row.nightHours = 0;
    }
  }

  // ── Time calculation ───────────────────────────────────────────────────────
  onTimeChange(row: AttendanceDayRow): void {
    if (row.isDayOff || !row.entry || !row.exit) {
      row.workedHours = 0;
      row.nightHours = 0;
      return;
    }

    const entryMin = this.timeToMinutes(row.entry);
    const exitMin = this.timeToMinutes(row.exit);

    if (entryMin === exitMin) {
      row.workedHours = 0;
      row.nightHours = 0;
      return;
    }

    // Worked hours (supports midnight crossing)
    const worked = exitMin > entryMin
      ? exitMin - entryMin
      : (1440 - entryMin) + exitMin;
    row.workedHours = Math.round((worked / 60) * 100) / 100;

    // Night hours (21:00–06:00)
    row.nightHours = this.calcNightMinutes(entryMin, exitMin) / 60;
    row.nightHours = Math.round(row.nightHours * 100) / 100;
  }

  private timeToMinutes(time: string): number {
    const [h, m] = time.split(':').map(Number);
    return (h || 0) * 60 + (m || 0);
  }

  private calcNightMinutes(entry: number, exit: number): number {
    const nightStart = 21 * 60; // 21:00
    const nightEnd = 6 * 60;    // 06:00
    let nightMin = 0;

    if (exit > entry) {
      // Same-day shift
      for (let m = entry; m < exit; m++) {
        if (m >= nightStart || m < nightEnd) nightMin++;
      }
    } else {
      // Midnight-crossing shift
      for (let m = entry; m < 1440; m++) {
        if (m >= nightStart) nightMin++;
      }
      for (let m = 0; m < exit; m++) {
        if (m < nightEnd) nightMin++;
      }
    }
    return nightMin;
  }

  // ── Computed totals ────────────────────────────────────────────────────────
  get totalWorkedHours(): number {
    return this.rows.reduce((s, r) => s + r.workedHours, 0);
  }

  get totalNightHours(): number {
    return this.rows.reduce((s, r) => s + r.nightHours, 0);
  }

  get totalExtraHours(): number {
    return Math.max(0, this.totalWorkedHours - 160);
  }

  get monthLabel(): string {
    return `${this.months[this.selectedMonth]} ${this.selectedYear}`;
  }

  // ── Actions ────────────────────────────────────────────────────────────────
  saveSheet(): void {
    if (!this.guardName.trim() || !this.workplace.trim()) {
      this.msgSvc.add({ severity: 'warn', summary: 'Atención', detail: 'Completá Apellido/Nombre y Objetivo.' });
      return;
    }
    this.saving = true;
    // TODO: Connect to backend service
    setTimeout(() => {
      this.saving = false;
      this.msgSvc.add({ severity: 'success', summary: 'Guardado', detail: 'Planilla de asistencia guardada correctamente.' });
    }, 600);
  }

  clearSheet(): void {
    this.guardName = '';
    this.workplace = '';
    this.buildRows();
    this.msgSvc.add({ severity: 'info', summary: 'Limpiado', detail: 'Planilla limpiada.' });
  }

  printSheet(): void {
    window.print();
  }

  // ── Helpers ────────────────────────────────────────────────────────────────
  formatHours(value: number): string {
    return value.toFixed(2);
  }
}
