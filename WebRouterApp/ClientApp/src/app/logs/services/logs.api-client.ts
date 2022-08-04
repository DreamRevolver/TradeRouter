import { Injectable } from '@angular/core';
import { ApiClient, ApiClientFactory } from '../../core/services/api/api.client';
import { ILogRecordModel } from '../models/log-record.model';

@Injectable({ providedIn: 'root' })
export class LogsApiClient {
  constructor(apiClientFactory: ApiClientFactory) {
    this.apiClient = apiClientFactory.forFeature('logs');
  }

  private readonly apiClient: ApiClient;

  async getAll(): Promise<ILogRecordModel[]> {
    return await this.apiClient.post<ILogRecordModel[]>(`logs`, {});
  }
}
