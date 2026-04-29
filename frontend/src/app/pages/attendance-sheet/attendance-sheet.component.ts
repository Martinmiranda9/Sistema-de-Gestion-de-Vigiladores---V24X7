import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

import { MessageService } from 'primeng/api';
import type { AttendanceSheetRow, SecurityGuard, Workplace, AttendanceSheetCreatePayload } from '../../core/models';
import { WorkplaceService } from '../../core/services/workplace.service';
import { SecurityGuardService } from '../../core/services/security-guard.service';
import { AttendanceSheetService } from '../../core/services/attendance-sheet.service';
import { GeminiService } from '../../core/services/gemini.service';

@Component({
  selector: 'app-attendance-sheet',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    ButtonModule, ToastModule, TooltipModule,
    ProgressSpinnerModule
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

  // OCR State
  ocrFile: File | null = null;
  ocrPreviewUrl: string | null = null;
  ocrProcessing = false;
  isDragOver = false;

  constructor(
    private router: Router,
    private msgSvc: MessageService,
    private workplaceSvc: WorkplaceService,
    private guardSvc: SecurityGuardService,
    private attendanceSvc: AttendanceSheetService,
    private geminiSvc: GeminiService
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
    const daysInMonth = new Date(this.selectedYear, this.selectedMonth + 1, 0).getDate();
    
    // Si la planilla estaba vacía, la creamos de cero
    if (!this.rows || this.rows.length === 0) {
      this.buildRows();
      return;
    }

    // Si ya tiene datos, solo ajustamos la cantidad de días sin borrar lo existente
    if (this.rows.length < daysInMonth) {
      // Agregar días faltantes
      for (let i = this.rows.length; i < daysInMonth; i++) {
        this.rows.push({
          day: i + 1,
          entry: '',
          exit: '',
          isDayOff: false,
          workedHours: 0,
          nightHours: 0,
          notes: ''
        });
      }
    } else if (this.rows.length > daysInMonth) {
      // Quitar días sobrantes
      this.rows.splice(daysInMonth);
    }
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
    const nightStart = 21 * 60;
    const nightEnd = 6 * 60;
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
    this.clearOcr();
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

  // ─── OCR Methods ───────────────────────────────────────────────

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.setOcrFile(input.files[0]);
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = false;
    const files = event.dataTransfer?.files;
    if (files && files[0] && files[0].type.startsWith('image/')) {
      this.setOcrFile(files[0]);
    } else {
      this.msgSvc.add({ severity: 'warn', summary: 'Archivo inválido', detail: 'Por favor, soltá una imagen (JPG, PNG, WEBP).' });
    }
  }

  private setOcrFile(file: File): void {
    this.ocrFile = file;
    const reader = new FileReader();
    reader.onload = () => { this.ocrPreviewUrl = reader.result as string; };
    reader.readAsDataURL(file);
  }

  clearOcr(): void {
    this.ocrFile = null;
    this.ocrPreviewUrl = null;
  }

  async processOcr(): Promise<void> {
    if (!this.ocrFile) {
      this.msgSvc.add({ severity: 'warn', summary: 'Sin imagen', detail: 'Primero seleccioná una imagen de la planilla.' });
      return;
    }

    this.ocrProcessing = true;
    try {
      const result = await this.geminiSvc.processAttendanceSheet(this.ocrFile);

      // ── Mapear cabecera (Período) ────────────────────────────────────
      if (result.month != null) {
        const monthIndex = this.parseMonth(result.month);
        if (monthIndex >= 0) this.selectedMonth = monthIndex;
      }
      if (result.year) {
        this.selectedYear = result.year;
      }

      // ── Mapear Objetivo (Workplace) ──────────────────────────────────
      const matchedMessages: string[] = [];

      if (result.workplace) {
        const matchedWp = this.findBestMatch(
          result.workplace,
          this.workplaces,
          wp => wp.name
        );
        if (matchedWp) {
          this.selectedWorkplaceId = matchedWp.id;
          matchedMessages.push(`Objetivo: ${matchedWp.name}`);

          // Cargar vigiladores del objetivo seleccionado y luego matchear
          if (result.guardName) {
            await this.loadGuardsAndMatch(matchedWp.id, result.guardName, matchedMessages);
          } else {
            // Solo cargar los vigiladores sin matchear
            await this.loadGuardsAsync(matchedWp.id);
          }
        } else {
          matchedMessages.push(`Objetivo "${result.workplace}" no encontrado en el sistema`);
        }
      }

      // ── Si no se matcheó workplace pero sí hay guardName, buscar en TODOS ──
      if (!this.selectedWorkplaceId && result.guardName) {
        await this.searchGuardInAllWorkplaces(result.guardName, matchedMessages);
      }

      // ── Mapear filas ─────────────────────────────────────────────────
      if (result.rows && result.rows.length > 0) {
        this.buildRows();

        result.rows.forEach(ocrRow => {
          const target = this.rows.find(r => r.day === ocrRow.day);
          if (!target) return;

          target.entry    = this.normalizeTime(ocrRow.entry) ?? '';
          target.exit     = this.normalizeTime(ocrRow.exit) ?? '';
          target.isDayOff = ocrRow.isDayOff ?? false;
          target.notes    = ocrRow.notes ?? '';

          if (target.isDayOff) {
            this.toggleDayOff(target);
          } else {
            this.onTimeChange(target);
          }
        });

        const filled = result.rows.filter(r => r.entry || r.exit || r.isDayOff).length;
        const detail = matchedMessages.length > 0
          ? `Se importaron ${filled} días. ${matchedMessages.join(' · ')}. Revisá los datos antes de guardar.`
          : `Se importaron ${filled} días. Revisá los datos antes de guardar.`;
        this.msgSvc.add({
          severity: 'success',
          summary: '¡Planilla leída!',
          detail,
          life: 8000
        });
      } else {
        this.msgSvc.add({
          severity: 'warn',
          summary: 'Sin datos',
          detail: 'La IA no encontró filas de datos en la imagen. Intentá con una imagen más clara.'
        });
      }
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Error desconocido al procesar la imagen.';
      this.msgSvc.add({
        severity: 'error',
        summary: 'Error de OCR',
        detail: msg.length > 200 ? msg.substring(0, 200) + '…' : msg,
        life: 8000
      });
    } finally {
      this.ocrProcessing = false;
    }
  }

  /** Carga los vigiladores de un objetivo y matchea por nombre */
  private loadGuardsAndMatch(workplaceId: number, guardName: string, messages: string[]): Promise<void> {
    return new Promise<void>(resolve => {
      this.loadingGuards = true;
      this.guardSvc.getByWorkplace(workplaceId).subscribe({
        next: (data: SecurityGuard[]) => {
          this.guards = data.filter((g: SecurityGuard) => g.isActive);
          this.loadingGuards = false;

          const matchedGuard = this.findBestMatch(
            guardName,
            this.guards,
            g => g.fullName || `${g.lastName}, ${g.firstName}`
          );
          if (matchedGuard) {
            this.selectedGuardId = matchedGuard.id;
            messages.push(`Vigilador: ${matchedGuard.fullName || matchedGuard.lastName + ', ' + matchedGuard.firstName}`);
          } else {
            messages.push(`Vigilador "${guardName}" no encontrado en este objetivo`);
          }
          resolve();
        },
        error: () => {
          this.loadingGuards = false;
          messages.push('No se pudieron cargar los vigiladores');
          resolve();
        }
      });
    });
  }

  /** Carga los vigiladores de un objetivo (sin matchear) */
  private loadGuardsAsync(workplaceId: number): Promise<void> {
    return new Promise<void>(resolve => {
      this.loadingGuards = true;
      this.guardSvc.getByWorkplace(workplaceId).subscribe({
        next: (data: SecurityGuard[]) => {
          this.guards = data.filter((g: SecurityGuard) => g.isActive);
          this.loadingGuards = false;
          resolve();
        },
        error: () => {
          this.loadingGuards = false;
          resolve();
        }
      });
    });
  }

  /** Busca un vigilador recorriendo todos los objetivos */
  private async searchGuardInAllWorkplaces(guardName: string, messages: string[]): Promise<void> {
    for (const wp of this.workplaces) {
      const guards = await new Promise<SecurityGuard[]>(resolve => {
        this.guardSvc.getByWorkplace(wp.id).subscribe({
          next: (data: SecurityGuard[]) => resolve(data.filter(g => g.isActive)),
          error: () => resolve([])
        });
      });

      const matched = this.findBestMatch(
        guardName,
        guards,
        g => g.fullName || `${g.lastName}, ${g.firstName}`
      );

      if (matched) {
        this.selectedWorkplaceId = wp.id;
        this.guards = guards;
        this.selectedGuardId = matched.id;
        messages.push(`Objetivo: ${wp.name}`);
        messages.push(`Vigilador: ${matched.fullName || matched.lastName + ', ' + matched.firstName}`);
        return;
      }
    }
    messages.push(`Vigilador "${guardName}" no encontrado en ningún objetivo`);
  }

  /** Convierte el mes devuelto por la IA (número o texto) al índice 0-based */
  private parseMonth(value: string | number): number {
    if (typeof value === 'number') return value - 1;
    const lower = value.toLowerCase();
    return this.months.findIndex(m => m.toLowerCase().startsWith(lower.substring(0, 3)));
  }

  /** Normaliza un string de hora al formato HH:mm, o devuelve null si está vacío */
  private normalizeTime(time: string | undefined): string | null {
    if (!time) return null;
    const clean = time.trim();
    if (!clean) return null;
    // Ya tiene formato HH:mm
    if (/^\d{1,2}:\d{2}$/.test(clean)) {
      const [h, m] = clean.split(':').map(Number);
      return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}`;
    }
    return null;
  }

  /**
   * Encuentra el mejor match entre una cadena de búsqueda y una lista de candidatos.
   * Usa normalización (sin tildes, minúsculas) y compara por palabras contenidas.
   */
  private findBestMatch<T>(search: string, items: T[], labelFn: (item: T) => string): T | null {
    if (!search || items.length === 0) return null;

    const normalized = this.normalize(search);
    if (normalized.length < 3) return null;

    const searchWords = normalized.split(/[\s,]+/).filter(w => w.length > 2);

    let bestItem: T | null = null;
    let bestScore = 0;

    for (const item of items) {
      const label = this.normalize(labelFn(item));

      // Si es exactamente igual, es el ganador indiscutido
      if (label === normalized) {
        return item;
      }

      // Match por palabras: contar cuántas palabras de búsqueda están contenidas
      let score = 0;
      for (const word of searchWords) {
        if (label.includes(word)) {
          score += word.length; // Ponderar por largo de la palabra
        }
      }

      // También comprobar si las palabras del label están en la búsqueda
      const labelWords = label.split(/[\s,]+/).filter(w => w.length > 2);
      for (const word of labelWords) {
        if (normalized.includes(word)) {
          score += word.length;
        }
      }

      if (score > bestScore) {
        bestScore = score;
        bestItem = item;
      }
    }

    // Requiere un score mínimo razonable (al menos un apellido de 3+ caracteres)
    return bestScore >= 4 ? bestItem : null;
  }

  /** Normaliza un string: minúsculas, sin tildes/diacríticos */
  private normalize(text: string): string {
    return text
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .trim();
  }
}
