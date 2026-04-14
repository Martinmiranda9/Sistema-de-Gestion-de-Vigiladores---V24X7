import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RippleModule } from 'primeng/ripple';
import { TooltipModule } from 'primeng/tooltip';

interface MenuItem {
  label: string;
  icon: string;
  route: string;
  badge?: number;
}

interface MenuSection {
  title: string;
  items: MenuItem[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, RippleModule, TooltipModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  @Input() collapsed = false;
  @Output() collapsedChange = new EventEmitter<boolean>();

  menuSections: MenuSection[] = [
    {
      title: 'Gestión',
      items: [
        { label: 'Dashboard', icon: 'pi pi-th-large', route: '/dashboard' },
        { label: 'Vigiladores', icon: 'pi pi-users', route: '/vigiladores' },
        { label: 'Objetivos', icon: 'pi pi-flag', route: '/objetivos' },
      ]
    },
    {
      title: 'Planilla',
      items: [
        { label: 'Horas Extras', icon: 'pi pi-clock', route: '/horas-extras' },
        { label: 'Calendario', icon: 'pi pi-calendar', route: '/calendario' },
        { label: 'Histórico', icon: 'pi pi-history', route: '/historico' },
      ]
    },
    {
      title: 'Liquidación',
      items: [
        { label: 'Hora Extra', icon: 'pi pi-dollar', route: '/hora-extra' },
        { label: 'Hora Nocturna', icon: 'pi pi-moon', route: '/hora-nocturna' },
        { label: 'Hora Feriada', icon: 'pi pi-star', route: '/hora-feriada' },
      ]
    }
  ];

  toggleCollapse(): void {
    this.collapsed = !this.collapsed;
    this.collapsedChange.emit(this.collapsed);
  }
}
