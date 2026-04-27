import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { MainLayoutComponent } from './shared/layout/main-layout.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { SecurityGuardsComponent } from './pages/security-guards/security-guards.component';
import { AddSecurityGuardComponent } from './pages/security-guards/add-security-guard/add-security-guard.component';
import { EditSecurityGuardComponent } from './pages/security-guards/edit-security-guard/edit-security-guard.component';
import { WorkplacesListComponent } from './pages/workplaces/workplaces-list/workplaces-list.component';
import { WorkplaceFormComponent } from './pages/workplaces/workplace-form/workplace-form.component';
import { OvertimeComponent } from './pages/overtime/overtime.component';
import { NightRateComponent } from './pages/night-rate/night-rate.component';
import { HolidayRateComponent } from './pages/holiday-rate/holiday-rate.component';
import { RateFormComponent } from './shared/components/rate-form/rate-form.component';
import { OvertimeSpreadsheetComponent } from './pages/overtime-spreadsheet/overtime-spreadsheet.component';
import { OvertimeHistoryComponent } from './pages/overtime-history/overtime-history.component';
import { AttendanceSheetComponent } from './pages/attendance-sheet/attendance-sheet.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'vigiladores', component: SecurityGuardsComponent },
      { path: 'vigiladores/nuevo', component: AddSecurityGuardComponent },
      { path: 'vigiladores/editar/:id', component: EditSecurityGuardComponent },
      { path: 'objetivos', component: WorkplacesListComponent },
      { path: 'objetivos/nuevo', component: WorkplaceFormComponent },
      { path: 'objetivos/editar/:id', component: WorkplaceFormComponent },
      // Planilla
      { path: 'planilla/horas-extras', component: OvertimeSpreadsheetComponent },
      { path: 'calendario', component: AttendanceSheetComponent },
      { path: 'historico', component: OvertimeHistoryComponent },
      // Liquidación — Hora Extra
      { path: 'horas-extras', component: OvertimeComponent },
      { path: 'horas-extras/actualizar', component: RateFormComponent },
      { path: 'horas-extras/programar', component: RateFormComponent },
      // Liquidación — Hora Nocturna
      { path: 'hora-nocturna', component: NightRateComponent },
      { path: 'hora-nocturna/actualizar', component: RateFormComponent },
      { path: 'hora-nocturna/programar', component: RateFormComponent },
      // Liquidación — Hora Feriada
      { path: 'hora-feriada', component: HolidayRateComponent },
      { path: 'hora-feriada/actualizar', component: RateFormComponent },
      { path: 'hora-feriada/programar', component: RateFormComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
