import { Guid } from '../../shared/types/guid';

export interface ISubscriberOverview {
  id: Guid;
  name: string;
  utcStartedAt: Date;
  ping: number;
  formattedBalance: string;
  description: string;
  multiplier: number;
  status: number;
}
