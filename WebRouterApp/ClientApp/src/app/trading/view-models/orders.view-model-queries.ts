import { Selector } from '@ngxs/store';
import { TradingState } from '../state/trading.state';
import { IOrder } from '../models/order';
import { Owned } from '../models/owned-entity';
import { ISubscriberOrderViewModel } from './subscriber-order.view-model';
import { DashboardPublishersState } from '../../dashboard/state/dashboard-publishers.state';
import { IPublisherOverview } from '../../dashboard/models/publisher-overview';
import { IPublisherOrderViewModel } from './publisher-order.view-model';
import { ISubscriberOverview } from '../../dashboard/models/subscriber-overview';
import { DashboardSubscribersState } from '../../dashboard/state/dashboard-subscribers.state';
import { PUBLISHER_ORDERS, SUBSCRIBERS_ORDERS } from '../models/fake-orders';

export class OrdersViewModelQueries {
  @Selector([DashboardPublishersState.publishers, TradingState.orders])
  static publisherOrders(
    publishers: IPublisherOverview[],
    orders: Owned<IOrder>[]
  ): IPublisherOrderViewModel[] {
    // return PUBLISHER_ORDERS;

    if (publishers.length === 0) {
      return [];
    }

    const publisher = publishers[0];

    const viewModels = orders
      .filter((it) => it.traderId === publisher.id)
      .map((it) => publisherOrderViewModelFrom(publisher, it));
    return viewModels;
  }

  @Selector([
    DashboardPublishersState.publishers,
    DashboardSubscribersState.subscribers,
    TradingState.orders,
  ])
  static subscriberOrders(
    publishers: IPublisherOverview[],
    subscribers: ISubscriberOverview[],
    orders: Owned<IOrder>[]
  ): ISubscriberOrderViewModel[] {
    // return SUBSCRIBERS_ORDERS;

    if (publishers.length === 0) {
      return [];
    }

    const publisher = publishers[0]; // TODO: This must be filtered by publisher.id when we have multiple publishers

    const viewModels = orders
      .filter((o) => o.traderId !== publisher.id)
      .map((o) =>
        subscriberOrderViewModelFrom(
          publisher,
          subscribers.find((s) => s.id === o.traderId) ?? null,
          orders,
          o
        )
      );
    return viewModels;
  }
}

function commonViewModelFrom(order: Owned<IOrder>) {
  const viewModel = {
    id: `${order.traderId}::${order.entity.clientId}`,
    opentime: new Date(),
    symbol: order.entity.symbol,
    side: order.entity.orderSide,
    status: order.entity.orderState,
    client_Id: order.entity.clientId,
    entryprice: order.entity.price,
    size: order.entity.amount,
  };

  return viewModel;
}

function publisherOrderViewModelFrom(
  publisher: IPublisherOverview,
  order: Owned<IOrder>
): IPublisherOrderViewModel {
  const commonProps = commonViewModelFrom(order);
  const viewModel: IPublisherOrderViewModel = {
    ...commonProps,
    node: publisher.name,
    unsynchronized: 0,
  };

  return viewModel;
}

function subscriberOrderViewModelFrom(
  publisher: IPublisherOverview,
  subscriber: ISubscriberOverview | null,
  orders: Owned<IOrder>[],
  order: Owned<IOrder>
): ISubscriberOrderViewModel {
  const commonProps = commonViewModelFrom(order);

  const pubOrder = orders.find(
    (it) =>
      it.traderId === publisher.id &&
      it.entity.symbol === order.entity.symbol &&
      it.entity.orderSide === order.entity.orderSide
  );

  const subMultiplier = subscriber?.multiplier ?? 1;
  const orderType = !pubOrder
    ? 'Manual'
    : pubOrder.entity.clientId === order.entity.clientId
    ? 'Auto'
    : 'Manual'; // If there is no such an order in the publisher's list then it is a manual order.
  const subPoints = commonProps.size / subMultiplier;
  const pubSize = pubOrder?.entity.amount ?? commonProps.size * subMultiplier;

  const diff = Math.abs(pubSize - subPoints);

  // TODO: This is threshold in % which defines what position difference will be considered as unsynced.
  //       This must become configurable
  const UNSYNC_THRESHOLD = 0.018;
  const unsynchronized = diff / Math.abs(commonProps.size) > UNSYNC_THRESHOLD ? 1 : 0;

  const viewModel: ISubscriberOrderViewModel = {
    ...commonProps,
    node: subscriber?.name || '???',
    follows: publisher.name,
    size: commonProps.size,
    points: subPoints,
    mid: 1,
    diff: diff,
    unsynchronized: unsynchronized,
    // TODO unsync > 0 ? 'Not synced!' : Math.random() > 0.9 ? 'Manual' : 'Auto',
    type: orderType,
  };

  return viewModel;
}
