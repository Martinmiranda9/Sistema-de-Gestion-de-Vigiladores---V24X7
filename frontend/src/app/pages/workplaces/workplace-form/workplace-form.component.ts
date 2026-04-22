import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageService, MenuItem } from 'primeng/api';
import { WorkplaceService } from '../../../core/services/workplace.service';
import { Workplace, WorkplaceCreate, WorkplaceUpdate } from '../../../core/models';

@Component({
  selector: 'app-workplace-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    ToastModule,
    BreadcrumbModule,
    ProgressSpinnerModule
  ],
  providers: [MessageService],
  templateUrl: './workplace-form.component.html',
  styleUrl: './workplace-form.component.css'
})
export class WorkplaceFormComponent implements OnInit {

  breadcrumbItems: MenuItem[] = [];
  breadcrumbHome: MenuItem = { icon: 'pi pi-home', routerLink: '/dashboard' };

  isEditing = false;
  workplaceId!: number;
  loadingData = false;
  saving = false;

  formData: WorkplaceUpdate = {
    name: '',
    address: '',
    isActive: true
  };

  originalData: WorkplaceUpdate = { ...this.formData };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private workplaceService: WorkplaceService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    // Determine if we are editing or creating based on route params
    this.route.paramMap.subscribe(params => {
      const idParam = params.get('id');
      if (idParam) {
        this.isEditing = true;
        this.workplaceId = +idParam;
        
        this.breadcrumbItems = [
          { label: 'Objetivos', routerLink: '/objetivos' },
          { label: 'Editar Objetivo' }
        ];

        this.loadWorkplaceData();
      } else {
        this.isEditing = false;
        
        this.breadcrumbItems = [
          { label: 'Objetivos', routerLink: '/objetivos' },
          { label: 'Nuevo Objetivo' }
        ];
      }
    });
  }

  loadWorkplaceData(): void {
    this.loadingData = true;
    this.workplaceService.getById(this.workplaceId).subscribe({
      next: (wp: Workplace) => {
        this.formData = {
          name: wp.name,
          address: wp.address,
          isActive: wp.isActive
        };
        this.originalData = { ...this.formData };
        this.loadingData = false;
      },
      error: () => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'No se pudo cargar la información del objetivo.'
        });
        this.router.navigate(['/objetivos']);
      }
    });
  }

  get isFormValid(): boolean {
    return !!(this.formData.name?.trim() && this.formData.address?.trim());
  }

  isFieldModified(field: keyof WorkplaceUpdate): boolean {
    if (!this.isEditing || this.loadingData) return false;
    return this.formData[field] !== this.originalData[field];
  }

  saveWorkplace(): void {
    if (!this.isFormValid) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Atención',
        detail: 'Completá los campos obligatorios (Nombre y Dirección).'
      });
      return;
    }

    this.saving = true;

    if (this.isEditing) {
      const payload: WorkplaceUpdate = {
        name: this.formData.name.trim(),
        address: this.formData.address.trim(),
        isActive: this.formData.isActive
      };

      this.workplaceService.update(this.workplaceId, payload).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Actualizado', detail: 'Objetivo actualizado correctamente.' });
          setTimeout(() => this.router.navigate(['/objetivos']), 1200);
        },
        error: (err: any) => {
          this.saving = false;
          this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'No se pudo actualizar el objetivo.' });
        }
      });
    } else {
      const payload: WorkplaceCreate = {
        name: this.formData.name.trim(),
        address: this.formData.address.trim()
      };

      this.workplaceService.create(payload).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Registrado', detail: 'Objetivo registrado correctamente.' });
          setTimeout(() => this.router.navigate(['/objetivos']), 1200);
        },
        error: (err: any) => {
          this.saving = false;
          this.messageService.add({ severity: 'error', summary: 'Error', detail: err.error?.message || 'No se pudo registrar el objetivo.' });
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/objetivos']);
  }
}
