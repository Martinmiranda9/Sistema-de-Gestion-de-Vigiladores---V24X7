import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';

import { MessageService } from 'primeng/api';
import type { AttendanceSheetRow, SecurityGuard, Workplace, AttendanceSheetCreatePayload } from '../../core/models';
import { WorkplaceService } from '../../core/services/workplace.service';
import { SecurityGuardService } from '../../core/services/security-guard.service';
import { AttendanceSheetService } from '../../core/services/attendance-sheet.service';

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
export class AttendanceSheetComponent implements OnInit {

  months = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
  ];
  selectedMonth: number;
  selectedYear: number;
  
  workplaces: Workplace[] = [];
  selectedWorkplaceId: number | null = null;
  loadingWorkplaces = true;

  guards: SecurityGuard[] = [];
  selectedGuardId: number | null = null;
  loadingGuards = false;

  rows: AttendanceSheetRow[] = [];

  saving = false;

  constructor(
    private router: Router,
    private msgSvc: MessageService,
    private workplaceSvc: WorkplaceService,
    private guardSvc: SecurityGuardService,
    private attendanceSvc: AttendanceSheetService
  ) {
    const now = new Date();
    this.selectedMonth = now.getMonth();
    this.selectedYear = now.getFullYear();
    this.buildRows();
  }

  ngOnInit(): void {
    this.loadWorkplaces();
  }

  loadWorkplaces(): void {
    this.loadingWorkplaces = true;
    this.workplaceSvc.getAll().subscribe({
      next: (data: Workplace[]) => {
        this.workplaces = data.filter((w: Workplace) => w.isActive);
        this.loadingWorkplaces = false;
      },
      error: (_err: unknown) => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los objetivos.' });
        this.loadingWorkplaces = false;
      }
    });
  }

  onWorkplaceChange(): void {
    this.selectedGuardId = null;
    this.guards = [];
    if (!this.selectedWorkplaceId) return;

    this.loadingGuards = true;
    this.guardSvc.getByWorkplace(this.selectedWorkplaceId).subscribe({
      next: (data: SecurityGuard[]) => {
        this.guards = data.filter((g: SecurityGuard) => g.isActive);
        this.loadingGuards = false;
      },
      error: (_err: unknown) => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los vigiladores del objetivo.' });
        this.loadingGuards = false;
      }
    });
  }

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

  toggleDayOff(row: AttendanceSheetRow): void {
    if (row.isDayOff) {
      row.entry = '';
      row.exit = '';
      row.workedHours = 0;
      row.nightHours = 0;
    }
  }

  onTimeChange(row: AttendanceSheetRow): void {
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
      for (let m = entry; m < exit; m++) {
        if (m >= nightStart || m < nightEnd) nightMin++;
      }
    } else {
      for (let m = entry; m < 1440; m++) {
        if (m >= nightStart) nightMin++;
      }
      for (let m = 0; m < exit; m++) {
        if (m < nightEnd) nightMin++;
      }
    }
    return nightMin;
  }

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

  saveSheet(): void {
    if (!this.selectedWorkplaceId || !this.selectedGuardId) {
      this.msgSvc.add({ severity: 'warn', summary: 'Atención', detail: 'Seleccioná un Objetivo y un Vigilador.' });
      return;
    }

    if (this.totalWorkedHours === 0) {
      this.msgSvc.add({ severity: 'warn', summary: 'Atención', detail: 'La planilla no tiene horas registradas.' });
      return;
    }

    const payload: AttendanceSheetCreatePayload = {
      securityGuardId: this.selectedGuardId,
      workplaceId: this.selectedWorkplaceId,
      month: this.selectedMonth + 1,
      year: this.selectedYear,
      totalWorkedHours: this.totalWorkedHours,
      totalNightHours: this.totalNightHours,
      totalExtraHours: this.totalExtraHours,
      rows: this.rows.map(r => ({
        day: r.day,
        entry: r.entry,
        exit: r.exit,
        isDayOff: r.isDayOff,
        workedHours: r.workedHours,
        nightHours: r.nightHours,
        notes: r.notes
      }))
    };

    this.saving = true;
    this.attendanceSvc.create(payload).subscribe({
      next: () => {
        this.saving = false;
        this.msgSvc.add({ severity: 'success', summary: 'Guardado', detail: 'Planilla guardada correctamente.' });
        setTimeout(() => this.router.navigate(['/calendario']), 1000);
      },
      error: (err: { error?: { message?: string } }) => {
        this.saving = false;
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'No se pudo guardar la planilla.' });
      }
    });
  }

  clearSheet(): void {
    this.selectedWorkplaceId = null;
    this.selectedGuardId = null;
    this.guards = [];
    this.buildRows();
  }

  goBack(): void {
    this.router.navigate(['/calendario']);
  }

  printSheet(): void {
    window.print();
  }

  formatHours(value: number): string {
    return value.toFixed(2);
  }
}
