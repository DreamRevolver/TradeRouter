import { Selector } from '@ngxs/store';
import {
  IPublisherOverviewViewModel,
  PublishersViewModelQueries,
} from './publisher-overview.view-model-queries';
import { TraderStatus } from '../models/trader-status';
import { filterTraders } from './utils';
import { ITraderOverviewViewModel } from './trader-overview.view-model';
import { ISubscriberOverview } from '../models/subscriber-overview';
import { DashboardSubscribersState } from '../state/dashboard-subscribers.state';
import { TradingState } from '../../trading/state/trading.state';
import { Owned } from '../../trading/models/owned-entity';
import { IOrder } from '../../trading/models/order';
import { IPosition } from '../../trading/models/position';

export interface ISubscriberOverviewViewModel extends ITraderOverviewViewModel {
  publisherOrders: number;
  publisherPositions: number;
}

export class SubscriberOverviewViewModelQueries {
  @Selector([
    PublishersViewModelQueries.publishers,
    DashboardSubscribersState.subscribers,
    TradingState.orders,
    TradingState.positions,
  ])
  static subscribers(
    publishers: IPublisherOverviewViewModel[],
    subscribers: ISubscriberOverview[],
    orders: Owned<IOrder>[],
    positions: Owned<IPosition>[]
  ): ISubscriberOverviewViewModel[] {
    if (publishers.length === 0) {
      return [];
    }
    const publisher = publishers[0];
    const viewModels = subscribers.map((it, i) =>
      createViewModel(publisher, it, i, orders, positions)
    );
    return viewModels;
  }

  @Selector([SubscriberOverviewViewModelQueries.subscribers])
  static runningSubscribersCount(subscribers: ISubscriberOverviewViewModel[]): number {
    return filterTraders(subscribers, TraderStatus.Running).length;
  }

  @Selector([SubscriberOverviewViewModelQueries.subscribers])
  static pausedSubscribersCount(subscribers: ISubscriberOverviewViewModel[]): number {
    return filterTraders(subscribers, TraderStatus.Paused).length;
  }

  @Selector([SubscriberOverviewViewModelQueries.subscribers])
  static stoppedSubscribersCount(subscribers: ISubscriberOverviewViewModel[]): number {
    return filterTraders(subscribers, TraderStatus.Stopped).length;
  }

  @Selector([SubscriberOverviewViewModelQueries.subscribers])
  static errorCount(subscribers: ISubscriberOverviewViewModel[]): number {
    return subscribers.reduce((count, it) => count + it.errors, 0);
  }
}

function createViewModel(
  publisher: IPublisherOverviewViewModel,
  subscriber: ISubscriberOverview,
  i: number,
  orders: Owned<IOrder>[],
  positions: Owned<IPosition>[]
): ISubscriberOverviewViewModel {
  const viewModel: ISubscriberOverviewViewModel = {
    id: subscriber.id,
    node: subscriber.name,
    ping: subscriber.ping,
    uptime: subscriber.utcStartedAt ? Date.now() - subscriber.utcStartedAt.getTime() : 0,
    formattedBalance:
      subscriber.formattedBalance === 'No USDT wallet' ? '--' : subscriber.formattedBalance,
    publisherOrders: publisher.orders,
    publisherPositions: publisher.positions,
    orders: orders.filter((it) => it.traderId === subscriber.id).length,
    positions: positions.filter((it) => it.traderId === subscriber.id).length,
    description: subscriber.description,
    status: TraderStatus.stringify(subscriber.status),
    // Fake data
    errors: i % 5 === 0 ? Math.round(Math.random() > 0.55 ? (Math.random() % 0.2) * 10 : 0) : 0,
  };

  return viewModel;
}
