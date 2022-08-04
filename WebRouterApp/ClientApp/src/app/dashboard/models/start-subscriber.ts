import { Guid } from '../../shared/types/guid';
import { TraderStatus } from './trader-status';

export interface IStartSubscriberApiResponse {
  readonly statusOverviews: ISubscriberStatusOverview[];
}

export interface ISubscriberStatusOverview {
  readonly id: Guid;
  readonly status: TraderStatus;
}
