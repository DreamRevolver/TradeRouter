import { Injectable } from '@angular/core';
import { ApiClient, ApiClientFactory } from '../../core/services/api/api.client';
import { Guid } from '../../shared/types/guid';

@Injectable({ providedIn: 'root' })
export class TradingApiClient {
  constructor(apiClientFactory: ApiClientFactory) {
    this.apiClient = apiClientFactory.forFeature('trading');
  }

  private readonly apiClient: ApiClient;

  async requestTradingSnapshot(connectionId: string): Promise<void> {
    return this.apiClient.post<void>(`requestTradingSnapshot`, { connectionId });
  }

  async cancelAllOrdersOfSubscribers(ids?: Guid[]): Promise<void> {
    return this.apiClient.post<void>('cancelAllOrdersOfSubscribers', { subscriberIds: ids });
  }

  async closeAllPositionsOfSubscribers(ids?: Guid[]): Promise<void> {
    return this.apiClient.post<void>('closeAllPositionsOfSubscribers', { subscriberIds: ids });
  }

  async syncOrdersOfSubscribers(ids?: Guid[]): Promise<void> {
    return this.apiClient.post<void>('syncOrdersOfSubscribers', { subscriberIds: ids });
  }

  async syncPositionsOfSubscribers(ids?: Guid[]): Promise<void> {
    return this.apiClient.post<void>('syncPositionsOfSubscribers', { subscriberIds: ids });
  }
}
