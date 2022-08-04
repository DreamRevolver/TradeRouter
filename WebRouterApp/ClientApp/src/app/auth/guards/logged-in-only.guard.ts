import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';
import { Store } from '@ngxs/store';
import { AuthState } from '../state/auth.state';
import { AuthActions } from '../state/auth.actions';

@Injectable({ providedIn: 'root' })
export class LoggedInOnlyGuard implements CanActivate {
  constructor(private store: Store) {}

  canActivate(_: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const isLoggedIn = this.store.selectSnapshot(AuthState.isLoggedIn);
    if (!isLoggedIn) {
      this.store.dispatch(new AuthActions.RedirectToLogin({ originalUrl: state.url }));
      return false;
    }

    return true;
  }
}
