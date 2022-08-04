import { Injectable } from '@angular/core';
import { ApiClient, ApiClientFactory } from '../../core/services/api/api.client';
import { IPublisherOverview } from '../models/publisher-overview';
import { Guid } from '../../shared/types/guid';

@Injectable({ providedIn: 'root' })
export class PublishersApiClient {
  private readonly apiClient: ApiClient;

  constructor(apiClientFactory: ApiClientFactory) {
    this.apiClient = apiClientFactory.forFeature('publishers');
  }

  async getOverviews(ids?: Guid[]): Promise<IPublisherOverview[]> {
    return this.apiClient.post<IPublisherOverview[]>(`getOverviews`, { publisherIds: ids });
  }
}
