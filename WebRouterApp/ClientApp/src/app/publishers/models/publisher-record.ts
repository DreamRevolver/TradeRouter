import { Guid } from '../../shared/types/guid';

export interface IPublisherRecord {
  id: Guid;
  name: string;
  description: string;
  apiKey: string;
  apiSecret: string;
  tradeAllOrdersAsMarket: boolean;
}
