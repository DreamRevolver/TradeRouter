import { Owned } from './owned-entity';

export interface IOrder {
  symbol: string;
  orderSide: string;
  orderState: string;
  price: number;
  amount: number; // TODO maybe change it to size because it is confusing naming
  clientId: string;
}

export function equalOrders(left: Owned<IOrder>, right: Owned<IOrder>): boolean {
  return left.traderId === right.traderId && left.entity.clientId === right.entity.clientId;
}

export function ordersContain(orders: Owned<IOrder>[], ownedOrder: Owned<IOrder>): boolean {
  return orders.some((it) => equalOrders(it, ownedOrder));
}
