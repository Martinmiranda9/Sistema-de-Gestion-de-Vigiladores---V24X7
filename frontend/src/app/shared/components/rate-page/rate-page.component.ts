import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { TimelineModule } from 'primeng/timeline';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';

import { MessageService, ConfirmationService } from 'primeng/api';

import { PayrollConfig, TimelineEvent } from '../../../core/models';

/** Tipo de tarifa que maneja este componente */
export type RateField = 'extraHourRate' | 'nightSurchargeRate' | 'holidayHourRate';

@Component({
  selector: 'app-rate-page',
  standalone: true,
  imports: [
    CommonModule, FormsModule, RouterModule,
    ButtonModule, InputNumberModule,
    TimelineModule, ToastModule, ConfirmDialogModule,
    TagModule, TooltipModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './rate-page.component.html',
  styleUrl: './rate-page.component.css'
})
export class RatePageComponent implements OnInit, OnChanges {

  // ── Configuración ────────────────────────────────────────────────────────
  @Input() pageTitle = 'Tarifa';
  @Input() pageSubtitle = 'Gestioná el valor vigente, programá aumentos y consultá el historial';
  @Input() rateField: RateField = 'extraHourRate';
  @Input() rateLabel = 'hora extra';
  @Input() iconClass = 'pi pi-clock';
  @Input() updateRoute = '/horas-extras/actualizar';
  @Input() scheduleRoute = '/horas-extras/programar';
  @Input() simLabel = 'Simulá el monto de horas extras';

  /** Datos crudos del backend ya cargados por el padre */
  @Input() configs: PayrollConfig[] = [];
  @Input() loading = true;

  @Output() deleteRequest = new EventEmitter<number>();

  // ── State derivado ────────────────────────────────────────────────────────
  currentConfig: PayrollConfig | null = null;
  upcoming: PayrollConfig | null = null;
  history: TimelineEvent[] = [];

  // ── Simulator ─────────────────────────────────────────────────────────────
  simHours = 0;
  simResult = 0;

  constructor(
    private msgSvc: MessageService,
    private confirmSvc: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.processConfigs();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['configs'] || changes['rateField']) {
      this.processConfigs();
    }
  }

  private processConfigs(): void {
    if (!this.configs.length) return;
    const today = new Date();

    const sorted = [...this.configs].sort((a, b) => {
      const validDiff = new Date(b.validFrom).getTime() - new Date(a.validFrom).getTime();
      if (validDiff === 0) {
        return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
      }
      return validDiff;
    });

    this.currentConfig = sorted.find(c => new Date(c.validFrom) <= today) ?? null;

    const upcomingList = sorted.filter(c => new Date(c.validFrom) > today);
    this.upcoming = upcomingList.length > 0 ? upcomingList[upcomingList.length - 1] : null;

    // Filtrar: solo mostrar registros donde ESTA tarifa realmente cambió
    // Recorremos de más viejo a más nuevo y detectamos cambios
    const chronological = [...sorted].reverse(); // más viejo primero
    const relevantIds = new Set<number>();

    // El primero siempre es relevante
    if (chronological.length > 0) {
      relevantIds.add(chronological[0].id);
    }

    for (let i = 1; i < chronological.length; i++) {
      const prevRate = (chronological[i - 1] as any)[this.rateField] as number;
      const currRate = (chronological[i] as any)[this.rateField] as number;
      if (currRate !== prevRate) {
        relevantIds.add(chronological[i].id);
      }
    }

    // También incluir siempre el vigente actual y los futuros
    if (this.currentConfig) relevantIds.add(this.currentConfig.id);
    upcomingList.forEach(c => relevantIds.add(c.id));

    const filtered = sorted.filter(c => relevantIds.has(c.id));

    this.history = filtered.map(c => ({
      id: c.id,
      validFrom: new Date(c.validFrom),
      createdAt: new Date(c.createdAt),
      rate: (c as any)[this.rateField] as number,
      reason: c.reason,
      changedBy: c.changedBy,
      isCurrent: this.currentConfig?.id === c.id,
      isUpcoming: new Date(c.validFrom) > today
    }));
  }

  getRate(config: PayrollConfig): number {
    return (config as any)[this.rateField] as number;
  }

  // ── Delete ─────────────────────────────────────────────────────────────────
  confirmDelete(event: TimelineEvent): void {
    if (event.isCurrent) {
      this.msgSvc.add({ severity: 'warn', summary: 'Atención', detail: 'No podés eliminar el valor vigente.' });
      return;
    }
    this.confirmSvc.confirm({
      message: '¿Estás seguro de que querés eliminar este registro del historial?',
      header: 'Confirmar eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, eliminar',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.deleteRequest.emit(event.id)
    });
  }

  // ── Cancel upcoming ────────────────────────────────────────────────────────
  confirmCancelUpcoming(): void {
    if (!this.upcoming) return;
    this.confirmSvc.confirm({
      message: `¿Cancelás el aumento programado para el ${this.formatDate(this.upcoming.validFrom)}?`,
      header: 'Cancelar aumento programado',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, cancelar',
      rejectLabel: 'No',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.deleteRequest.emit(this.upcoming!.id)
    });
  }

  // ── Simulator ─────────────────────────────────────────────────────────────
  calculate(): void {
    const rate = this.currentConfig ? this.getRate(this.currentConfig) : 0;
    this.simResult = this.simHours > 0 ? this.simHours * rate : 0;
  }

  // ── Helpers (compartidos) ─────────────────────────────────────────────────
  formatCurrency(value: number): string {
    return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS', maximumFractionDigits: 2 }).format(value);
  }

  formatDate(date: Date | string): string {
    return new Intl.DateTimeFormat('es-AR', { day: '2-digit', month: 'long', year: 'numeric' }).format(new Date(date));
  }

  daysUntil(date: Date | string): number {
    const diff = new Date(date).getTime() - new Date().getTime();
    return Math.ceil(diff / (1000 * 60 * 60 * 24));
  }
}
