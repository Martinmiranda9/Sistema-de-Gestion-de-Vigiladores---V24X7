import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { OvertimeSpreadsheetService, OvertimeSpreadsheetDetail, OvertimeSpreadsheetSummary } from '../../core/services/overtime-spreadsheet.service';

@Component({
  selector: 'app-overtime-history',
  standalone: true,
  imports: [CommonModule, FormsModule, ToastModule, ButtonModule],
  providers: [MessageService],
  templateUrl: './overtime-history.component.html',
  styleUrl: './overtime-history.component.css'
})
export class OvertimeHistoryComponent implements OnInit {
  history: OvertimeSpreadsheetSummary[] = [];
  selectedSpreadsheet: OvertimeSpreadsheetDetail | null = null;
  loading = false;
  loadingDetail = false;

  searchText = '';
  selectedMonth: number | null = null;
  selectedYear: number;

  months = [
    { label: 'Todos los meses', value: null as number | null },
    { label: 'Enero', value: 1 },
    { label: 'Febrero', value: 2 },
    { label: 'Marzo', value: 3 },
    { label: 'Abril', value: 4 },
    { label: 'Mayo', value: 5 },
    { label: 'Junio', value: 6 },
    { label: 'Julio', value: 7 },
    { label: 'Agosto', value: 8 },
    { label: 'Septiembre', value: 9 },
    { label: 'Octubre', value: 10 },
    { label: 'Noviembre', value: 11 },
    { label: 'Diciembre', value: 12 }
  ];

  constructor(
    private overtimeSpreadsheetSvc: OvertimeSpreadsheetService,
    private msgSvc: MessageService
  ) {
    this.selectedYear = new Date().getFullYear();
  }

  ngOnInit(): void {
    this.loadHistory();
  }

  loadHistory(): void {
    this.loading = true;
    this.selectedSpreadsheet = null;

    this.overtimeSpreadsheetSvc.getHistory(this.selectedMonth, this.selectedYear, this.searchText).subscribe({
      next: (data) => {
        this.history = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo cargar el histórico.' });
      }
    });
  }

  openSpreadsheet(id: number): void {
    this.loadingDetail = true;
    this.overtimeSpreadsheetSvc.getById(id).subscribe({
      next: (data) => {
        this.selectedSpreadsheet = data;
        this.loadingDetail = false;
      },
      error: () => {
        this.loadingDetail = false;
        this.msgSvc.add({ severity: 'error', summary: 'Error', detail: 'No se pudo abrir la planilla seleccionada.' });
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS', maximumFractionDigits: 2 }).format(value);
  }

  formatDate(value: string): string {
    return new Date(value).toLocaleDateString('es-AR');
  }

  get monthLabel(): string {
    if (!this.selectedSpreadsheet) {
      return '';
    }
    return this.months.find(m => m.value === this.selectedSpreadsheet!.month)?.label ?? '';
  }
}
