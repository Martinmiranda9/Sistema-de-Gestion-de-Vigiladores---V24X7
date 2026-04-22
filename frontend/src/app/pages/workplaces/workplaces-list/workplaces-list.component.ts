import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TooltipModule } from 'primeng/tooltip';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { MessageService, ConfirmationService } from 'primeng/api';
import { WorkplaceService } from '../../../core/services/workplace.service';
import { Workplace } from '../../../core/models';

@Component({
  selector: 'app-workplaces-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    ToastModule,
    ConfirmDialogModule,
    TooltipModule,
    IconFieldModule,
    InputIconModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './workplaces-list.component.html',
  styleUrl: './workplaces-list.component.css'
})
export class WorkplacesListComponent implements OnInit {
  workplaces: Workplace[] = [];
  filteredWorkplaces: Workplace[] = [];
  loading = true;

  // Search and Filter
  searchText = '';
  statusFilter: 'all' | 'active' | 'inactive' = 'all';

  // Stats
  totalWorkplaces = 0;
  activeCount = 0;
  inactiveCount = 0;

  // Pagination
  currentPage = 1;
  rowsPerPage = 10;

  constructor(
    private workplaceService: WorkplaceService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadWorkplaces();
  }

  loadWorkplaces(): void {
    this.loading = true;
    this.workplaceService.getAll().subscribe({
      next: (data: Workplace[]) => {
        this.workplaces = data;
        this.updateStats();
        this.applyFilter();
        this.loading = false;
      },
      error: (err: any) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los objetivos.' });
        this.loading = false;
      }
    });
  }

  updateStats(): void {
    this.totalWorkplaces = this.workplaces.length;
    this.activeCount = this.workplaces.filter(w => w.isActive).length;
    this.inactiveCount = this.workplaces.filter(w => !w.isActive).length;
  }

  filterByStatus(status: 'all' | 'active' | 'inactive'): void {
    this.statusFilter = status;
    this.applyFilter();
  }

  applyFilter(): void {
    const searchLower = this.searchText.toLowerCase().trim();
    this.filteredWorkplaces = this.workplaces.filter(wp => {
      // 1. Text filter
      const matchesSearch = wp.name.toLowerCase().includes(searchLower) ||
                            wp.address.toLowerCase().includes(searchLower);

      // 2. Status filter
      let matchesStatus = true;
      if (this.statusFilter === 'active') {
        matchesStatus = wp.isActive;
      } else if (this.statusFilter === 'inactive') {
        matchesStatus = !wp.isActive;
      }

      return matchesSearch && matchesStatus;
    });

    this.currentPage = 1;
  }

  get paginatedWorkplaces(): Workplace[] {
    const startIndex = (this.currentPage - 1) * this.rowsPerPage;
    return this.filteredWorkplaces.slice(startIndex, startIndex + this.rowsPerPage);
  }

  get totalPages(): number {
    return Math.ceil(this.filteredWorkplaces.length / this.rowsPerPage) || 1;
  }

  get showingFrom(): number {
    return this.filteredWorkplaces.length === 0 ? 0 : (this.currentPage - 1) * this.rowsPerPage + 1;
  }

  get showingTo(): number {
    return Math.min(this.currentPage * this.rowsPerPage, this.filteredWorkplaces.length);
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

  goToNew(): void {
    this.router.navigate(['/objetivos/nuevo']);
  }

  goToEdit(wp: Workplace): void {
    this.router.navigate(['/objetivos/editar', wp.id]);
  }

  confirmDelete(wp: Workplace): void {
    this.confirmationService.confirm({
      message: `¿Estás seguro de que querés eliminar el objetivo "${wp.name}"?`,
      header: 'Confirmar Eliminación',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sí, eliminar',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.workplaceService.delete(wp.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Eliminado', detail: 'Objetivo eliminado correctamente.' });
            this.loadWorkplaces();
          },
          error: (err: any) => {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar el objetivo.' });
          }
        });
      }
    });
  }
}
