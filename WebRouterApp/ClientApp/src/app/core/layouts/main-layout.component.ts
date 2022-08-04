import { Component } from '@angular/core';

@Component({
  selector: 'app-main-layout',
  template: `
    <app-toolbar (toggleSidenav)="sideNav.toggle()"></app-toolbar>
    <app-global-error></app-global-error>
    <app-sidenav #sideNav>
      <router-outlet></router-outlet>
    </app-sidenav>
  `,
})
export class MainLayoutComponent {}
