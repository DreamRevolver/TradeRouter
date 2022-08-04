import { Component, EventEmitter, Output } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { AuthActions } from '../../auth/state/auth.actions';
import { AuthState } from '../../auth/state/auth.state';
import { Observable } from 'rxjs';
import { IUserModel } from '../../users/models/user.model';
import { RoutePaths } from '../constants/routes-paths';

@Component({
  selector: 'app-toolbar',
  template: `
    <mat-toolbar color="primary">
      <button mat-button class="sidenav-toggle" (click)="toggleSidenav.emit()">
        <mat-icon>menu</mat-icon>
      </button>

      <span>Trade Router</span>

      <span class="flex-spacer"></span>
      <span *ngIf="currentUser$ | async as currentUser">
        <button mat-button [matMenuTriggerFor]="currentUserMenu">
          <mat-icon>account_circle</mat-icon>
          <span class="user-name">
            {{ currentUser?.firstName }}&nbsp;{{ currentUser?.lastName }}
          </span>
        </button>
        <mat-menu #currentUserMenu="matMenu">
          <button mat-menu-item (click)="logOut()">Log out</button>
        </mat-menu>
      </span>
    </mat-toolbar>
  `,
  styles: [
    `
      $iconWidth: 56px;

      .sidenav-toggle {
        display: flex;
        align-items: center;
        justify-content: center;

        padding: 0;
        margin: 8px 8px 8px -14px;
        min-width: $iconWidth;

        .user-name {
          padding: 0;
          margin: 8px;
        }

        mat-icon {
          font-size: 30px;
          height: $iconWidth;
          width: $iconWidth;
          line-height: $iconWidth;
          color: white;
        }
      }
    `,
  ],
})
export class ToolbarComponent {
  @Output() toggleSidenav = new EventEmitter<void>();

  constructor(private store: Store) {}

  @Select(AuthState.currentUser)
  readonly currentUser$!: Observable<IUserModel | null>;

  logOut(): void {
    this.store.dispatch([
      new AuthActions.LogOut(),
      new AuthActions.RedirectToLogin({ originalUrl: RoutePaths.empty }),
    ]);
  }
}
