import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RippleModule } from 'primeng/ripple';
import { BadgeModule } from 'primeng/badge';

import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RippleModule,
    BadgeModule,

    InputTextModule
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  @Input() sidebarCollapsed = false;
  @Output() toggleSidebar = new EventEmitter<void>();

  searchActive = false;
  userName = 'Maria Moreno';
  userRole = 'Administrador';

  toggleSearch(): void {
    this.searchActive = !this.searchActive;
  }
}
