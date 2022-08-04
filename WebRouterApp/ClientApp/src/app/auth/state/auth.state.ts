import { Injectable } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { IUserModel } from '../../users/models/user.model';
import { AuthApiClient } from '../services/auth.api-client';
import { AuthActions } from './auth.actions';
import { Router } from '@angular/router';
import { RoutePaths } from '../../core/constants/routes-paths';
import { isInvalidCredentialsError } from '../models/login';
import { isHttpErrorResponse } from '../../core/services/api/api.client';
import { AppActions } from '../../core/state/app/app.actions';
import { TradingHubActions } from '../../trading/state/trading.actions';
import { hasActionsExecuting } from '@ngxs-labs/actions-executing';

export interface AuthStateModel {
  defaults: Partial<AuthStateModel>;
  currentUser: IUserModel | null;
  token: string | null;
  originalUrl: string | null;
  lastError: string | null;
}

const defaults = {
  currentUser: null,
  token: null,
  originalUrl: null,
  lastError: null,
};

@State<AuthStateModel>({
  name: AuthState.featureName,
  defaults: {
    ...defaults,
    defaults: defaults,
  },
})
@Injectable()
export class AuthState {
  constructor(private router: Router, private authApi: AuthApiClient) {}

  static featureName = 'auth';
  static storageKeys = [
    `auth.currentUser`, //
    `auth.token`,
  ];

  @Selector()
  static currentUser(stateModel: AuthStateModel): IUserModel | null {
    return stateModel.currentUser;
  }

  @Selector()
  static token(stateModel: AuthStateModel): string | null {
    return stateModel.token;
  }

  @Selector([AuthState.currentUser])
  static isLoggedIn(currentUser: IUserModel | null): boolean {
    return !!currentUser;
  }

  @Selector()
  static lastError(stateModel: AuthStateModel): string | null {
    return stateModel.lastError;
  }

  @Selector([hasActionsExecuting([AuthActions.LogIn])])
  static isBusy(hasActionsExecuting: boolean): boolean {
    return hasActionsExecuting;
  }

  @Action(AuthActions.RedirectToLogin)
  protected async redirectToLogin(
    context: StateContext<AuthStateModel>,
    action: AuthActions.RedirectToLogin
  ): Promise<void> {
    if (action.payload.originalUrl) {
      context.patchState({
        originalUrl: action.payload.originalUrl,
      });
    }

    await this.router.navigate([RoutePaths.login]);
  }

  @Action(AuthActions.RedirectToDefault)
  protected async redirectToDefault(): Promise<void> {
    await this.router.navigate([RoutePaths.empty]);
  }

  @Action(AuthActions.LogIn)
  protected async login(
    context: StateContext<AuthStateModel>,
    action: AuthActions.LogIn
  ): Promise<void> {
    try {
      const originalUrl = context.getState().originalUrl;

      const response = await this.authApi.login(action.payload);
      context.patchState({
        currentUser: response.user,
        token: response.accessToken,
        originalUrl: null,
        lastError: null,
      });

      context.dispatch(new AppActions.DismissError());
      await context.dispatch(new TradingHubActions.Start()).toPromise();
      await this.router.navigate([originalUrl || RoutePaths.empty]);
    } catch (error) {
      context.patchState({
        currentUser: null,
        token: null,
      });

      if (isHttpErrorResponse(error) && isInvalidCredentialsError(error.error)) {
        // We report invalid credentials withing login control,
        // i.e. we handle this error locally.
        context.patchState({ lastError: error.error.message });
        return;
      }

      // We let everything else bubble up to the global handler.
      throw error;
    }
  }

  @Action(AuthActions.RefreshToken)
  protected async refreshToken(
    context: StateContext<AuthStateModel>,
    action: AuthActions.RefreshToken
  ): Promise<void> {
    context.patchState({
      currentUser: action.payload.user,
      token: action.payload.accessToken,
    });
  }

  @Action(AuthActions.LogOut)
  protected async logout(context: StateContext<AuthStateModel>): Promise<void> {
    // Another portion of the logout-related code is located in logout.plugin.ts

    await context.dispatch(new TradingHubActions.Stop()).toPromise();

    context.patchState({
      currentUser: null,
      token: null,
      originalUrl: null,
      lastError: null,
    });
  }
}
