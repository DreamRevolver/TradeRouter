import { Injectable } from '@angular/core';
import { ApiClient, ApiClientFactory } from '../../core/services/api/api.client';
import { ISubscriberOverview } from '../models/subscriber-overview';
import { Guid } from '../../shared/types/guid';
import { ISubscriberRecord } from '../../subscribers/models/subscriber-record';
import { IStartSubscriberApiResponse } from '../models/start-subscriber';

@Injectable({ providedIn: 'root' })
export class SubscribersApiClient {
  private readonly apiClient: ApiClient;

  constructor(apiClientFactory: ApiClientFactory) {
    this.apiClient = apiClientFactory.forFeature('subscribers');
  }

  async getOverviews(ids?: Guid[]): Promise<ISubscriberOverview[]> {
    return this.apiClient.post<ISubscriberOverview[]>(`getOverviews`, { subscriberIds: ids });
  }

  async getRecords(): Promise<ISubscriberRecord[]> {
    return this.apiClient.post<ISubscriberRecord[]>(`getRecords`, {});
  }

  async start(ids?: Guid[]): Promise<IStartSubscriberApiResponse> {
    return this.apiClient.post<IStartSubscriberApiResponse>(`start`, { subscriberIds: ids });
  }

  async pause(ids?: Guid[]): Promise<void> {
    return this.apiClient.post(`pause`, { subscriberIds: ids });
  }

  async stop(ids?: Guid[]): Promise<void> {
    return this.apiClient.post(`stop`, { subscriberIds: ids });
  }
}
