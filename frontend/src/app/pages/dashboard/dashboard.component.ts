import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { RippleModule } from 'primeng/ripple';
import { HttpClient, HttpHeaders } from '@angular/common/http';

interface KpiCard {
  title: string;
  value: string;
  subtitle: string;
  icon: string;
  trend: 'up' | 'down' | 'neutral';
  trendValue: string;
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

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    const token = localStorage.getItem('token') || sessionStorage.getItem('token');
    
    if (!token) {
      this.router.navigate(['/login']);
      return;
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    this.http.get<any[]>('http://localhost:8080/api/SecurityGuards', { headers }).subscribe({
      next: (guards) => {
        const index = this.kpiCards.findIndex(kpi => kpi.title === 'Vigiladores Activos');
        if (index !== -1) {
          this.kpiCards[index].value = guards.length.toString();
        }
      },
      error: (error) => {
        console.error('Error al cargar vigiladores', error);
        if (error.status === 401) {
          this.router.navigate(['/login']);
        }
      }
    });
  }

  kpiCards: KpiCard[] = [
    {
      title: 'Vigiladores Activos',
      value: '48',
      subtitle: 'Total registrados',
      icon: 'pi pi-users',
      trend: 'up',
      trendValue: '+3 este mes',
      color: '#2563eb'
    },
    {
      title: 'Hora Extra',
      value: '$4.850',
      subtitle: 'Valor actual por hora',
      icon: 'pi pi-clock',
      trend: 'up',
      trendValue: '+5.2%',
      color: '#059669'
    },
    {
      title: 'Hora Nocturna',
      value: '$5.620',
      subtitle: 'Valor actual por hora',
      icon: 'pi pi-moon',
      trend: 'neutral',
      trendValue: 'Sin cambios',
      color: '#7c3aed'
    },
    {
      title: 'Hora Feriada',
      value: '$7.275',
      subtitle: 'Valor actual por hora',
      icon: 'pi pi-star',
      trend: 'up',
      trendValue: '+8.1%',
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
