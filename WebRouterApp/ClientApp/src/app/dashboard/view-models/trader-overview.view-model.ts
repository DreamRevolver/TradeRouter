import { Guid } from '../../shared/types/guid';

export interface ITraderOverviewViewModel {
  id: Guid;
  node: string;
  uptime: number;
  ping: number;
  formattedBalance: string;
  orders: number;
  positions: number;
  description: string;
  errors: number;
  status: string;
}
