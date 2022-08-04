import { IOrder } from '../order';
import { TradingMessageTags } from './trading-message-tags';
import { Guid } from '../../../shared/types/guid';

export interface IOrdersSnapshotMessage {
  tag: TradingMessageTags.OrdersSnapshot;
  traderId: Guid;
  orders: IOrder[];
}
