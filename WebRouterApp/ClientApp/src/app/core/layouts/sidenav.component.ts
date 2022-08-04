import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ScopedSubscription } from '../../shared/scoped-subscription/scoped-subscription';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { RoutePaths } from '../constants/routes-paths';

@Component({
  selector: 'app-sidenav',
  animations: [
    trigger('openClose', [
      state('true', style({ width: '200px' })),
      state('false', style({ width: '60px' })),
      transition('false <=> true', animate('0.25s ease-in-out')),
    ]),
    trigger('visibleHidden', [
      transition(':enter', [
        style({ opacity: 0 }), //
        animate('0.25s ease-in-out', style({ opacity: 1 })),
      ]),
      transition(':leave', [
        style({ opacity: 1 }), //
        animate('0.25s ease-in-out', style({ opacity: 0 })),
      ]),
    ]),
  ],
  template: `
    <div class="sidenav-container">
      <div
        @visibleHidden
        *ngIf="isOpen"
        class="sidenav-backdrop"
        (click)="backdropClicked(); $event.stopPropagation()"
      ></div>

      <div
        [@openClose]="isOpen"
        class="sidenav"
        [ngStyle]="{
          width: isOpen ? '200px' : '60px',
          borderRight: isOpen ? '0' : 'solid 1px rgba(0, 0, 0, 0.25)'
        }"
      >
        <mat-nav-list>
          <a
            mat-list-item
            *ngFor="let navItem of navigationItems"
            [routerLink]="[navItem.route]"
            routerLinkActive="active-link"
          >
            <i mat-list-icon class="fa {{ navItem.icon }}"></i>
            <p matLine>{{ navItem.name }}</p>
          </a>
        </mat-nav-list>
      </div>

      <div class="sidenav-content">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [
    `
      .sidenav-container {
        position: relative;
        min-width: 100%;
        min-height: 100%;
      }

      .sidenav-backdrop {
        position: absolute;
        top: 0;
        right: 0;
        bottom: 0;
        left: 0;
        z-index: 4;
        background: rgba(0, 0, 0, 0.6);
      }

      .sidenav {
        position: absolute;
        top: 0;
        left: 0;
        bottom: 0;
        z-index: 4;
        background: #fff;
      }

      ::ng-deep .mat-list-item-content {
        padding: 0 12px !important;
      }

      .active-link {
        color: #3f51b5;
      }

      .sidenav-content {
        padding: 10px 50px 10px 110px;
      }
    `,
  ],
})
export class SideNavComponent implements OnInit {
  @Input() isOpen = false;

  constructor(private ss: ScopedSubscription, private router: Router) {}

  readonly navigationItems = [
    { name: 'Dashboard', icon: 'fa-desktop', route: RoutePaths.dashboard },
    { name: 'Trading', icon: 'fa-chart-line', route: RoutePaths.trading },
    { name: 'Publisher', icon: 'fa-brain', route: RoutePaths.publisherRecords },
    { name: 'Subscribers', icon: 'fa-users', route: RoutePaths.subscriberRecords },
    { name: 'Logs', icon: 'fa-book', route: RoutePaths.logs },
  ];

  ngOnInit(): void {
    this.ss.on(this.router.events, () => this.close());
  }

  readonly toggle = (): void => {
    this.isOpen = !this.isOpen;
  };

  readonly backdropClicked = (): void => {
    this.close();
  };

  private readonly close = (): void => {
    this.isOpen = false;
  };
}
