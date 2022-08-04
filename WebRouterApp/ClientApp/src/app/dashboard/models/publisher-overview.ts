import { Guid } from '../../shared/types/guid';

export interface IPublisherOverview {
  id: Guid;
  name: string;
  utcStartedAt: Date;
  ping: number;
  formattedBalance: string;
  description: string;
  status: number;
}
