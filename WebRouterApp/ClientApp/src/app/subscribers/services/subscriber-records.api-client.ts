import { Injectable } from '@angular/core';
import { ApiClient, ApiClientFactory } from '../../core/services/api/api.client';
import { ISubscriberRecord } from '../models/subscriber-record';
import { Guid } from '../../shared/types/guid';

@Injectable({ providedIn: 'root' })
export class SubscriberRecordsApiClient {
  constructor(apiClientFactory: ApiClientFactory) {
    this.apiClient = apiClientFactory.forFeature('subscribers');
  }

  private readonly apiClient: ApiClient;

  async getRecords(): Promise<ISubscriberRecord[]> {
    return this.apiClient.post<ISubscriberRecord[]>(`getRecords`, {});
  }

  async add(subscriber: ISubscriberRecord): Promise<{ id: Guid }> {
    return this.apiClient.post<{ id: Guid }>(`create`, { ...subscriber });
  }

  async update(subscriber: Partial<ISubscriberRecord>): Promise<void> {
    return this.apiClient.post(`update`, { subscriberId: subscriber.id, ...subscriber });
  }

  async delete(id: Guid): Promise<void> {
    return this.apiClient.post(`delete`, { subscriberId: id });
  }
}
