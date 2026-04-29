import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';

import { MessageService } from 'primeng/api';
import type { AttendanceSheet, Workplace, SecurityGuard } from '../../../core/models';
import { AttendanceSheetService } from '../../../core/services/attendance-sheet.service';
import { WorkplaceService } from '../../../core/services/workplace.service';
import { SecurityGuardService } from '../../../core/services/security-guard.service';

@Component({
  selector: 'app-attendance-history',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, ToastModule, TooltipModule],
  providers: [MessageService],
  templateUrl: './attendance-history.component.html',
  styleUrl: './attendance-history.component.css'
})
export class AttendanceHistoryComponent implements OnInit {

  months = [
    { value: 0, label: 'Todos los meses' },
    { value: 1, label: 'Enero' }, { value: 2, label: 'Febrero' },
    { value: 3, label: 'Marzo' }, { value: 4, label: 'Abril' },
    { value: 5, label: 'Mayo' }, { value: 6, label: 'Junio' },
    { value: 7, label: 'Julio' }, { value: 8, label: 'Agosto' },
    { value: 9, label: 'Septiembre' }, { value: 10, label: 'Octubre' },
    { value: 11, label: 'Noviembre' }, { value: 12, label: 'Diciembre' }
  ];

  monthNames = ['', 'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];


  workplaces: Workplace[] = [];
  guards: SecurityGuard[] = [];
  selectedWorkplaceId: number | null = null;
  selectedGuardId: number | null = null;
  selectedMonth: number = 0;
  selectedYear: number | null = new Date().getFullYear();
  searchText = '';


  history: AttendanceSheet[] = [];
  filteredHistory: AttendanceSheet[] = [];
  selectedSheet: AttendanceSheet | null = null;


  loading = false;
  loadingDetail = false;
  loadingWorkplaces = false;
  loadingGuards = false;

  constructor(
    private router: Router,
    private msgSvc: MessageService,
    private attendanceSvc: AttendanceSheetService,
    private workplaceSvc: WorkplaceService,
    private guardSvc: SecurityGuardService
  ) {}

  ngOnInit(): void {
    this.loadWorkplaces();
    this.loadHistory();
  }

  loadWorkplaces(): void {
    this.loadingWorkplaces = true;
    this.workplaceSvc.getAll().subscribe({
      next: (data: Workplace[]) => {
        this.workplaces = data.filter((w: Workplace) => w.isActive);
        this.loadingWorkplaces = false;
      },
      error: (_e: unknown) => { this.loadingWorkplaces = false; }
    });
  }

  onWorkplaceFilter(): void {
    this.selectedGuardId = null;
    this.guards = [];
    if (this.selectedWorkplaceId) {
      this.loadingGuards = true;
      this.guardSvc.getByWorkplace(this.selectedWorkplaceId).subscribe({
        next: (data: SecurityGuard[]) => { this.guards = data; this.loadingGuards = false; },
        error: (_e: unknown) => { this.loadingGuards = false; }
      });
    }
    this.loadHistory();
  }

  loadHistory(): void {
    this.loading = true;
    this.selectedSheet = null;

    const month = this.selectedMonth > 0 ? this.selectedMonth : undefined;
    const workplace = this.selectedWorkplaceId ?? undefined;
    const guard = this.selectedGuardId ?? undefined;
    const year = this.selectedYear ? this.selectedYear : undefined;

    this.attendanceSvc.getAll(workplace, guard, month, year).subscribe({
      next: (data: AttendanceSheet[]) => {
        this.history = data;
        this.applyTextFilter();
        this.loading = false;
      },
      error: (_e: unknown) => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo cargar el histórico.' });
        this.loading = false;
      }
    });
  }

  applyTextFilter(): void {
    if (!this.searchText.trim()) {
      this.filteredHistory = [...this.history];
      return;
    }
    const q = this.searchText.toLowerCase();
    this.filteredHistory = this.history.filter(s =>
      s.securityGuardName.toLowerCase().includes(q) ||
      s.workplaceName.toLowerCase().includes(q) ||
      s.securityGuardDNI.toLowerCase().includes(q)
    );
  }

  openDetail(sheet: AttendanceSheet): void {
    if (this.selectedSheet?.id === sheet.id) {
      this.selectedSheet = null;
      return;
    }
    this.loadingDetail = true;
    this.selectedSheet = null;
    this.attendanceSvc.getById(sheet.id).subscribe({
      next: (data: AttendanceSheet) => {
        this.selectedSheet = data;
        this.loadingDetail = false;
      },
      error: (_e: unknown) => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo abrir la planilla.' });
        this.loadingDetail = false;
      }
    });
  }

  deleteSheet(sheet: AttendanceSheet): void {
    if (!confirm(`¿Eliminar la planilla de ${sheet.securityGuardName} (${this.monthNames[sheet.month]} ${sheet.year})?`)) return;
    this.attendanceSvc.delete(sheet.id).subscribe({
      next: () => {
        this.msgSvc.add({ severity: 'success', summary: 'Eliminado', detail: 'Planilla eliminada correctamente.' });
        if (this.selectedSheet?.id === sheet.id) this.selectedSheet = null;
        this.loadHistory();
      },
      error: (_e: unknown) => {
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar la planilla.' });
      }
    });
  }

  goToNewSheet(): void {
    this.router.navigate(['/calendario/nueva']);
  }


  monthLabel(m: number): string {
    return this.monthNames[m] || '';
  }

  formatDate(iso: string): string {
    const d = new Date(iso);
    return d.toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }

  formatHours(v: number): string {
    return (v ?? 0).toFixed(2);
  }

  getDailyExtraHours(row: any): number {
    return Math.max(0, (row.workedHours || 0) - 8);
  }

  getDailyHolidayHours(row: any): number {

    return 0;
  }
}
