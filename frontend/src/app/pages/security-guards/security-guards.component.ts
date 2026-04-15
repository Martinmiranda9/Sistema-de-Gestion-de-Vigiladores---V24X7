import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { MessageService, ConfirmationService } from 'primeng/api';
import { SecurityGuardService, SecurityGuard, SecurityGuardCreate, SecurityGuardUpdate } from '../../core/services/security-guard.service';

@Component({
  selector: 'app-security-guards',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    TagModule,
    ToastModule,
    ConfirmDialogModule,
    TooltipModule,
    IconFieldModule,
    InputIconModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './security-guards.component.html',
  styleUrl: './security-guards.component.css'
})
export class SecurityGuardsComponent implements OnInit {
  guards: SecurityGuard[] = [];
  filteredGuards: SecurityGuard[] = [];
  workplaceOptions: { label: string; value: number }[] = [];
  loading = true;
  searchValue = '';
  statusFilter: 'all' | 'active' | 'inactive' = 'all';

  // Pagination
  currentPage = 1;
  rowsPerPage = 10;
  totalPages = 1;

  // Sorting
  sortField = '';
  sortOrder: 'asc' | 'desc' = 'asc';



  // Stats
  totalGuards = 0;
  activeGuards = 0;
  inactiveGuards = 0;

  constructor(
    private guardService: SecurityGuardService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadGuards();
  }

  loadGuards(): void {
    this.loading = true;
    this.guardService.getAll().subscribe({
      next: (data) => {
        this.guards = data;
        this.totalGuards = data.length;
        this.activeGuards = data.filter(g => g.isActive).length;
        this.inactiveGuards = data.filter(g => !g.isActive).length;

        // Extraer workplaces únicos para el dropdown del formulario
        const uniqueWorkplaces = new Map<number, string>();
        data.forEach(g => {
          if (g.workplaceId && g.workplaceName) {
            uniqueWorkplaces.set(g.workplaceId, g.workplaceName);
          }
        });
        this.workplaceOptions = Array.from(uniqueWorkplaces, ([value, label]) => ({ label, value }));

        this.applyFilter();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error al cargar vigiladores:', err);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los vigiladores.' });
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    const query = this.searchValue.toLowerCase().trim();

    // Start with all guards or filter by status
    let result = [...this.guards];
    if (this.statusFilter === 'active') {
      result = result.filter(g => g.isActive);
    } else if (this.statusFilter === 'inactive') {
      result = result.filter(g => !g.isActive);
    }

    // Then apply text search
    if (query) {
      result = result.filter(g =>
        g.firstName.toLowerCase().includes(query) ||
        g.lastName.toLowerCase().includes(query) ||
        g.dni.toLowerCase().includes(query) ||
        g.fileNumber.toLowerCase().includes(query) ||
        (g.workplaceName || '').toLowerCase().includes(query) ||
        g.fullName.toLowerCase().includes(query)
      );
    }

    this.filteredGuards = result;

    if (this.sortField) {
      this.applySorting();
    }

    this.totalPages = Math.max(1, Math.ceil(this.filteredGuards.length / this.rowsPerPage));
    if (this.currentPage > this.totalPages) {
      this.currentPage = 1;
    }
  }

  filterByStatus(status: 'all' | 'active' | 'inactive'): void {
    this.statusFilter = status;
    this.currentPage = 1;
    this.applyFilter();
  }

  onSearch(event: Event): void {
    this.searchValue = (event.target as HTMLInputElement).value;
    this.currentPage = 1;
    this.applyFilter();
  }

  // Sorting
  sortBy(field: string): void {
    if (this.sortField === field) {
      this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortField = field;
      this.sortOrder = 'asc';
    }
    this.applySorting();
  }

  private applySorting(): void {
    const field = this.sortField as keyof SecurityGuard;
    this.filteredGuards.sort((a, b) => {
      const valA = (a[field] || '').toString().toLowerCase();
      const valB = (b[field] || '').toString().toLowerCase();
      const cmp = valA.localeCompare(valB);
      return this.sortOrder === 'asc' ? cmp : -cmp;
    });
  }

  // Pagination
  get paginatedGuards(): SecurityGuard[] {
    const start = (this.currentPage - 1) * this.rowsPerPage;
    return this.filteredGuards.slice(start, start + this.rowsPerPage);
  }

  get showingFrom(): number {
    return this.filteredGuards.length === 0 ? 0 : (this.currentPage - 1) * this.rowsPerPage + 1;
  }

  get showingTo(): number {
    return Math.min(this.currentPage * this.rowsPerPage, this.filteredGuards.length);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  get pageNumbers(): number[] {
    const pages: number[] = [];
    const maxVisible = 5;
    let start = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
    let end = Math.min(this.totalPages, start + maxVisible - 1);
    start = Math.max(1, end - maxVisible + 1);
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }

  // CRUD Dialogs
  openNewDialog(): void {
    this.router.navigate(['/vigiladores/nuevo']);
  }

  openEditDialog(guard: SecurityGuard): void {
    this.router.navigate(['/vigiladores/editar', guard.id]);
  }

  confirmDelete(guard: SecurityGuard): void {
    this.confirmationService.confirm({
      message: `¿Estás seguro de que querés eliminar a ${guard.fullName}?`,
      header: 'Confirmar eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, eliminar',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.guardService.delete(guard.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Eliminado', detail: 'Vigilador eliminado correctamente.' });
            this.loadGuards();
          },
          error: (err) => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'No se pudo eliminar el vigilador.' });
          }
        });
      }
    });
  }
}
