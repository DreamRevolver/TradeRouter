import { Injectable } from '@angular/core';
import { ApiClient, ApiClientFactory } from '../../core/services/api/api.client';
import { IPublisherRecord } from '../models/publisher-record';

@Injectable({ providedIn: 'root' })
export class PublisherRecordsApiClient {
  constructor(apiClientFactory: ApiClientFactory) {
    this.apiClient = apiClientFactory.forFeature('publishers');
  }

  private readonly apiClient: ApiClient;

  async getRecords(): Promise<IPublisherRecord[]> {
    return this.apiClient.post<IPublisherRecord[]>(`getRecords`, {});
  }

  async update(publisher: Partial<IPublisherRecord>): Promise<void> {
    return this.apiClient.post(`update`, { publisherId: publisher.id, ...publisher });
  }
}
