import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { DividerModule } from 'primeng/divider';
import { MessageService, MenuItem } from 'primeng/api';
import { SecurityGuardService } from '../../../core/services/security-guard.service';
import { WorkplaceService } from '../../../core/services/workplace.service';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { SecurityGuardUpdate, Workplace } from '../../../core/models';

@Component({
  selector: 'app-edit-security-guard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    ToastModule,
    BreadcrumbModule,
    DividerModule,
    ProgressSpinnerModule
  ],
  providers: [MessageService],
  templateUrl: './edit-security-guard.component.html',
  styleUrl: './edit-security-guard.component.css'
})
export class EditSecurityGuardComponent implements OnInit {

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [];
  breadcrumbHome: MenuItem = { icon: 'pi pi-home', routerLink: '/dashboard' };

  // Workplaces dropdown
  workplaces: Workplace[] = [];
  workplaceOptions: { label: string; value: number }[] = [];

  guardId!: number;
  loadingData = true;
  saving = false;

  // Form
  formData: SecurityGuardUpdate = {
    firstName: '',
    lastName: '',
    dni: '',
    fileNumber: '',
    workplaceId: null,
    isActive: true
  };

  originalData: SecurityGuardUpdate = { ...this.formData };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private guardService: SecurityGuardService,
    private workplaceService: WorkplaceService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.breadcrumbItems = [
      { label: 'Vigiladores', routerLink: '/vigiladores' },
      { label: 'Editar Vigilador' }
    ];

    this.route.paramMap.subscribe(params => {
      const idParam = params.get('id');
      if (idParam) {
        this.guardId = +idParam;
        this.loadWorkplacesAndGuard();
      } else {
        this.router.navigate(['/vigiladores']);
      }
    });
  }

  loadWorkplacesAndGuard(): void {
    this.workplaceService.getAll().subscribe({
      next: (data) => {
        this.workplaces = data;
        this.workplaceOptions = data
          .filter(w => w.isActive)
          .map(w => ({ label: w.name, value: w.id }));

        this.loadGuardData();
      },
      error: () => {
        this.messageService.add({
          severity: 'warn',
          summary: 'Aviso',
          detail: 'No se pudieron cargar los objetivos.'
        });
        this.loadGuardData();
      }
    });
  }

  loadGuardData(): void {
    this.guardService.getById(this.guardId).subscribe({
      next: (guard) => {
        this.formData = {
          firstName: guard.firstName,
          lastName: guard.lastName,
          dni: guard.dni,
          fileNumber: guard.fileNumber,
          workplaceId: guard.workplaceId,
          isActive: guard.isActive
        };
        // Store a deep copy to compare later
        this.originalData = { ...this.formData };
        this.loadingData = false;
      },
      error: (err) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'No se pudo cargar la información del vigilador.'
        });
        this.router.navigate(['/vigiladores']);
      }
    });
  }

  get isFormValid(): boolean {
    return !!(
      this.formData.firstName?.trim() &&
      this.formData.lastName?.trim() &&
      this.formData.dni?.trim()
    );
  }

  isFieldModified(field: keyof SecurityGuardUpdate): boolean {
    if (this.loadingData) return false;
    return this.formData[field] !== this.originalData[field];
  }

  saveGuard(): void {
    if (!this.isFormValid) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Atención',
        detail: 'Completá los campos obligatorios (Nombre, Apellido y DNI).'
      });
      return;
    }

    this.saving = true;

    const payload: SecurityGuardUpdate = {
      firstName: this.formData.firstName.trim(),
      lastName: this.formData.lastName.trim(),
      dni: this.formData.dni.trim(),
      fileNumber: this.formData.fileNumber?.trim() || '',
      workplaceId: this.formData.workplaceId,
      isActive: this.formData.isActive
    };

    this.guardService.update(this.guardId, payload).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Actualizado',
          detail: 'Los datos del vigilador se actualizaron correctamente.'
        });
        // Small delay so the user sees the success toast
        setTimeout(() => this.router.navigate(['/vigiladores']), 1200);
      },
      error: (err) => {
        this.saving = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.message || 'No se pudo actualizar el vigilador.'
        });
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/vigiladores']);
  }
}
