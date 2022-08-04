import { IOrder } from '../order';
import { Guid } from '../../../shared/types/guid';
import { TradingMessageTags } from './trading-message-tags';

export interface IOrdersChangedMessage {
  tag: TradingMessageTags.OrdersChanged;
  traderId: Guid;
  changeSet: IOrdersChangeSet;
}

export interface IOrdersChangeSet {
  added: IOrder[];
  removed: IOrder[];
}
