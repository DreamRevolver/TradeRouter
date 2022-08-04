import { Injectable } from '@angular/core';
import { Action, Selector, State, StateContext, Store } from '@ngxs/store';
import { Router } from '@angular/router';
import { AppActions } from './app.actions';
import { MatSnackBar } from '@angular/material/snack-bar';
import { isRefreshTokenError } from '../../../auth/models/refresh-token.error';
import { is401Unauthorized } from '../../services/api/api.client';
import { AuthState } from '../../../auth/state/auth.state';
import { AuthActions } from '../../../auth/state/auth.actions';
import { RoutePaths } from '../../constants/routes-paths';

export interface AppStateModel {
  defaults: Partial<AppStateModel>;
  lastError: string | null;
}

const defaults = {
  lastError: null,
};

@State<AppStateModel>({
  name: AppState.featureName,
  defaults: {
    ...defaults,
    defaults: defaults,
  },
})
@Injectable()
export class AppState {
  constructor(private store: Store, private router: Router, private snackBar: MatSnackBar) {}

  static readonly featureName = 'app';
  private static readonly unexpectedErrorMessage =
    'We encountered an unexpected error. Please try again later.';

  @Selector()
  static lastError(stateModel: AppStateModel): unknown | null {
    return stateModel.lastError;
  }

  @Action(AppActions.HandleError)
  protected handleError(
    context: StateContext<AppStateModel>,
    action: AppActions.HandleError
  ): void {
    const error = action.payload.error;

    console.error('ERROR', error);

    if (this.tryHandleAuthError(context, error)) {
      return;
    }

    context.patchState({ lastError: AppState.unexpectedErrorMessage });

    // this.snackBar.open(message, 'DISMISS', { duration: 5000 });
  }

  private tryHandleAuthError(context: StateContext<AppStateModel>, error: unknown): boolean {
    if (!isRefreshTokenError(error) && !is401Unauthorized(error)) {
      // It's not an auth error, try another handler.
      return false;
    }

    if (isRefreshTokenError(error) && error.forDeferredRequest) {
      // It's an auth error, so don't try other handlers.
      // But in this case no further processing is required.
      return true;
    }

    const currentUrl = this.router.routerState.snapshot.url;
    const isLoggedIn = this.store.selectSnapshot(AuthState.isLoggedIn);

    if (isLoggedIn) {
      this.store.dispatch([
        new AuthActions.LogOut(),
        new AuthActions.RedirectToLogin({
          originalUrl: currentUrl !== RoutePaths.login ? currentUrl : RoutePaths.empty,
        }),
      ]);
    }

    if (currentUrl !== RoutePaths.login) {
      context.patchState({
        lastError: isLoggedIn
          ? 'For your security, please re-log in.'
          : 'Please log in to gain access to the application.',
      });

      // It's an auth error, don't try other handlers.
      return true;
    }

    context.patchState({ lastError: AppState.unexpectedErrorMessage });

    // It's an auth error, don't try other handlers.
    return true;
  }

  @Action(AppActions.DismissError)
  protected async dismissError(context: StateContext<AppStateModel>): Promise<void> {
    context.patchState({ lastError: null });
  }
}
