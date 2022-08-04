import { Injectable } from '@angular/core';
import { ApiClient, ApiClientFactory } from '../../core/services/api/api.client';
import { ILoginApiCommand, ILoginApiResponse } from '../models/login';
import { IRefreshTokenApiResponse } from '../models/refresh';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthApiClient {
  constructor(apiClientFactory: ApiClientFactory) {
    this.apiClient = apiClientFactory.forFeature(
      AuthApiClient.featureUrl, //
      { withCredentials: true }
    );
  }

  static featureUrl = 'auth';
  static loginUrl = 'login';
  static refreshUrl = 'refresh';

  private readonly apiClient: ApiClient;

  async login(command: ILoginApiCommand): Promise<ILoginApiResponse> {
    return await this.apiClient.post<ILoginApiResponse>(AuthApiClient.loginUrl, command);
  }

  refresh$(): Observable<IRefreshTokenApiResponse> {
    return this.apiClient.post$<IRefreshTokenApiResponse>(AuthApiClient.refreshUrl, {});
  }

  async logout(): Promise<boolean> {
    return true;
  }
}
