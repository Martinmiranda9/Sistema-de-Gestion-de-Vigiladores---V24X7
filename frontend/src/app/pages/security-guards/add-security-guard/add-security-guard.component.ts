import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { DividerModule } from 'primeng/divider';
import { MessageService, MenuItem } from 'primeng/api';
import { SecurityGuardService, SecurityGuardCreate } from '../../../core/services/security-guard.service';
import { WorkplaceService, Workplace } from '../../../core/services/workplace.service';

@Component({
  selector: 'app-add-security-guard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    InputTextModule,
    ToastModule,
    BreadcrumbModule,
    DividerModule
  ],
  providers: [MessageService],
  templateUrl: './add-security-guard.component.html',
  styleUrl: './add-security-guard.component.css'
})
export class AddSecurityGuardComponent implements OnInit {

  // Breadcrumb
  breadcrumbItems: MenuItem[] = [];
  breadcrumbHome: MenuItem = { icon: 'pi pi-home', routerLink: '/dashboard' };

  // Workplaces dropdown
  workplaces: Workplace[] = [];
  workplaceOptions: { label: string; value: number }[] = [];

  // Form
  formData: SecurityGuardCreate = {
    firstName: '',
    lastName: '',
    dni: '',
    fileNumber: '',
    workplaceId: null
  };

  saving = false;

  constructor(
    private guardService: SecurityGuardService,
    private workplaceService: WorkplaceService,
    private messageService: MessageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.breadcrumbItems = [
      { label: 'Vigiladores', routerLink: '/vigiladores' },
      { label: 'Nuevo Vigilador' }
    ];

    this.loadWorkplaces();
  }

  loadWorkplaces(): void {
    this.workplaceService.getAll().subscribe({
      next: (data) => {
        this.workplaces = data;
        this.workplaceOptions = data
          .filter(w => w.isActive)
          .map(w => ({ label: w.name, value: w.id }));
      },
      error: () => {
        this.messageService.add({
          severity: 'warn',
          summary: 'Aviso',
          detail: 'No se pudieron cargar los objetivos.'
        });
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

    const payload: SecurityGuardCreate = {
      firstName: this.formData.firstName.trim(),
      lastName: this.formData.lastName.trim(),
      dni: this.formData.dni.trim(),
      fileNumber: this.formData.fileNumber?.trim() || '',
      workplaceId: this.formData.workplaceId
    };

    this.guardService.create(payload).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Registrado',
          detail: `${payload.lastName}, ${payload.firstName} fue registrado correctamente.`
        });
        // Small delay so the user sees the success toast
        setTimeout(() => this.router.navigate(['/vigiladores']), 1200);
      },
      error: (err) => {
        this.saving = false;
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err.error?.message || 'No se pudo registrar el vigilador.'
        });
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/vigiladores']);
  }
}
