import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RippleModule } from 'primeng/ripple';
import { SecurityGuardService } from '../../core/services/security-guard.service';
import { PayrollConfigService } from '../../core/services/payroll-config.service';

interface KpiCard {
  title: string;
  value: string;
  subtitle: string;
  icon: string;
  lastUpdate: Date;
  color: string;
}

interface QuickAction {
  title: string;
  description: string;
  icon: string;
  route: string;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, RippleModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  currentDate = new Date();

  constructor(
    private securityGuardService: SecurityGuardService,
    private payrollConfigService: PayrollConfigService
  ) { }

  ngOnInit(): void {
    // 1. Cargar cantidad de vigiladores desde el servicio
    this.securityGuardService.getActiveGuardsCount().subscribe({
      next: (count) => {
        const index = this.kpiCards.findIndex(kpi => kpi.title === 'Vigiladores Activos');
        if (index !== -1) {
          this.kpiCards[index].value = count.toString();
        }
      },
      error: (error) => console.error('Error al cargar vigiladores', error)
    });

    // 2. Cargar configuraciones de pago desde el servicio
    this.payrollConfigService.getAll().subscribe({
      next: (configs) => {
        const today = new Date();
        const latestConf = configs
          .filter(c => new Date(c.validFrom) <= today)
          .sort((a, b) => {
            const validDiff = new Date(b.validFrom).getTime() - new Date(a.validFrom).getTime();
            if (validDiff === 0) {
              return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
            }
            return validDiff;
          })[0];

        if (latestConf) {
          const updateDate = new Date(latestConf.validFrom);

          const heIndex = this.kpiCards.findIndex(kpi => kpi.title === 'Hora Extra');
          if (heIndex !== -1) {
            this.kpiCards[heIndex].value = '$' + latestConf.extraHourRate.toLocaleString('es-AR');
            this.kpiCards[heIndex].lastUpdate = updateDate;
          }

          const hnIndex = this.kpiCards.findIndex(kpi => kpi.title === 'Hora Nocturna');
          if (hnIndex !== -1) {
            this.kpiCards[hnIndex].value = '$' + latestConf.nightSurchargeRate.toLocaleString('es-AR');
            this.kpiCards[hnIndex].lastUpdate = updateDate;
          }

          const hfIndex = this.kpiCards.findIndex(kpi => kpi.title === 'Hora Feriada');
          if (hfIndex !== -1) {
            this.kpiCards[hfIndex].value = '$' + latestConf.holidayHourRate.toLocaleString('es-AR');
            this.kpiCards[hfIndex].lastUpdate = updateDate;
          }
        }
      },
      error: (error: unknown) => console.error('Error al cargar configuraciones de pago', error)
    });
  }

  kpiCards: KpiCard[] = [
    {
      title: 'Vigiladores Activos',
      value: '-',
      subtitle: 'Total registrados',
      icon: 'pi pi-users',
      lastUpdate: new Date(),
      color: '#2563eb'
    },
    {
      title: 'Hora Extra',
      value: '$4.850',
      subtitle: 'Valor actual por hora',
      icon: 'pi pi-dollar',
      lastUpdate: new Date(2026, 3, 1), // 01/04/2026
      color: '#059669'
    },
    {
      title: 'Hora Nocturna',
      value: '$5.620',
      subtitle: 'Valor actual por hora',
      icon: 'pi pi-moon',
      lastUpdate: new Date(2026, 3, 1), // 01/04/2026
      color: '#7c3aed'
    },
    {
      title: 'Hora Feriada',
      value: '$7.275',
      subtitle: 'Valor actual por hora',
      icon: 'pi pi-star',
      lastUpdate: new Date(2026, 3, 1), // 01/04/2026
      color: '#ea580c'
    }
  ];

  quickActions: QuickAction[] = [
    {
      title: 'Agregar Vigilador',
      description: 'Ingresar datos para registrar un nuevo vigilador',
      icon: 'pi pi-user-plus',
      route: '/vigiladores',
      color: '#2563eb'
    },
    {
      title: 'Nuevo Objetivo',
      description: 'Crear un nuevo objetivo o punto de vigilancia',
      icon: 'pi pi-flag',
      route: '/objetivos',
      color: '#059669'
    },
    {
      title: 'Registrar Horas',
      description: 'Cargar horas extras, nocturnas o feriadas',
      icon: 'pi pi-clock',
      route: '/horas-extras',
      color: '#7c3aed'
    },
    {
      title: 'Ver Calendario',
      description: 'Consultar el calendario de turnos por objetivo',
      icon: 'pi pi-calendar',
      route: '/calendario',
      color: '#ea580c'
    },
    {
      title: 'Generar Liquidación',
      description: 'Calcular y exportar la planilla de liquidación',
      icon: 'pi pi-file-export',
      route: '/hora-extra',
      color: '#0891b2'
    },
    {
      title: 'Ver Histórico',
      description: 'Consultar registros anteriores y reportes',
      icon: 'pi pi-history',
      route: '/historico',
      color: '#be185d'
    }
  ];
}
