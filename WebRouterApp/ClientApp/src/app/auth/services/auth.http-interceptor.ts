import { Injectable, ModuleWithProviders, NgModule } from '@angular/core';
import {
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpEvent,
  HTTP_INTERCEPTORS,
} from '@angular/common/http';

import { BehaviorSubject, EMPTY, Observable, throwError } from 'rxjs';
import { catchError, filter, map, switchMap, take } from 'rxjs/operators';
import { Store } from '@ngxs/store';
import { is401Unauthorized } from '../../core/services/api/api.client';
import { AuthApiClient } from './auth.api-client';
import { AuthState } from '../state/auth.state';
import { IRefreshTokenApiResponse } from '../models/refresh';
import { AuthActions } from '../state/auth.actions';
import { IRefreshTokenError } from '../models/refresh-token.error';

@Injectable()
export class AuthHttpInterceptor implements HttpInterceptor {
  constructor(private store: Store, private authApi: AuthApiClient) {}

  private isRefreshing = false;
  private deferredRequestsSubject = new BehaviorSubject<string | null>(null);
  private deferredRequests$: Observable<string> = EMPTY;

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const token = this.store.selectSnapshot(AuthState.token);
    if (token != null) {
      request = AuthHttpInterceptor.setAuthHeader(request, token);
    }

    return next.handle(request).pipe(
      catchError((error) => {
        if (!this.shouldRefresh(request, error)) {
          // This error is not related to the access token expiring.
          // Just re-throw.
          return throwError(error);
        }

        if (!this.isRefreshing) {
          return this.beginRefresh(request, next);
        }

        return this.deferredRequests$.pipe(
          switchMap((accessToken) =>
            next.handle(AuthHttpInterceptor.setAuthHeader(request, accessToken))
          )
        );
      })
    );
  }

  private shouldRefresh(request: HttpRequest<unknown>, error: unknown): boolean {
    if (!is401Unauthorized(error)) {
      return false;
    }

    const loginUrl = AuthApiClient.featureUrl + '/' + AuthApiClient.loginUrl;
    const refreshUrl = AuthApiClient.featureUrl + '/' + AuthApiClient.refreshUrl;

    if (request.url.includes(loginUrl) || request.url.includes(refreshUrl)) {
      return false;
    }

    const isLoggedIn = this.store.selectSnapshot(AuthState.isLoggedIn);
    return isLoggedIn;
  }

  private beginRefresh(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    this.isRefreshing = true;

    // The previous behavior subject can be errored out, so we create a new one every time.
    this.deferredRequestsSubject = new BehaviorSubject<string | null>(null);
    this.deferredRequests$ = this.deferredRequestsSubject.pipe(
      filter((accessToken) => accessToken !== null),
      map((it) => it as string),
      take(1)
    );

    // We need to return this observable!
    // Otherwise nobody will subscribe to it.
    return this.authApi.refresh$().pipe(
      switchMap((response: IRefreshTokenApiResponse) => {
        this.isRefreshing = false;

        this.store.dispatch(new AuthActions.RefreshToken(response));
        this.deferredRequestsSubject.next(response.accessToken);

        return next.handle(AuthHttpInterceptor.setAuthHeader(request, response.accessToken));
      }),
      catchError((error) => {
        if (!this.isRefreshing) {
          // We might've refreshed the token successfully, and still ended up here
          // if the original API call resulted in an error.
          // In such a case, `this.isRefreshing` is already false,
          // and we want to just re-throw the error.
          return throwError(error);
        }

        this.isRefreshing = false;

        const refreshTokenError: IRefreshTokenError = {
          originalError: error,
          forDeferredRequest: false,
        };

        // "Fan out" the refresh token error to the deferred requests,
        // make sure the global error handling code has enough info to handle this error only once.
        const forDeferredRequests: IRefreshTokenError = {
          ...refreshTokenError,
          forDeferredRequest: true,
        };
        this.deferredRequestsSubject.error(forDeferredRequests);

        return throwError(refreshTokenError);
      })
    );
  }

  private static setAuthHeader(request: HttpRequest<unknown>, accessToken: string) {
    return request.clone({
      headers: request.headers.set('Authorization', `Bearer ${accessToken}`),
    });
  }
}

@NgModule()
export class AuthHttpInterceptorModule {
  public static forRoot(): ModuleWithProviders<AuthHttpInterceptorModule> {
    return {
      ngModule: AuthHttpInterceptorModule,
      providers: [
        {
          provide: HTTP_INTERCEPTORS,
          useClass: AuthHttpInterceptor,
          multi: true,
        },
      ],
    };
  }
}
