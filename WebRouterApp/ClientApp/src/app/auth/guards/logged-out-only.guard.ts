import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Store } from '@ngxs/store';
import { AuthState } from '../state/auth.state';
import { AuthActions } from '../state/auth.actions';

@Injectable({ providedIn: 'root' })
export class LoggedOutOnlyGuard implements CanActivate {
  constructor(private store: Store) {}

  canActivate(): boolean {
    const isLoggedIn = this.store.selectSnapshot(AuthState.isLoggedIn);
    if (isLoggedIn) {
      this.store.dispatch(new AuthActions.RedirectToDefault());
      return false;
    }

    return true;
  }
}
