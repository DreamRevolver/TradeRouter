import { Selector } from '@ngxs/store';
import { IPublisherOverview } from '../models/publisher-overview';
import { TraderStatus } from '../models/trader-status';
import { filterTraders } from './utils';
import { ITraderOverviewViewModel } from './trader-overview.view-model';
import { DashboardPublishersState } from '../state/dashboard-publishers.state';
import { TradingState } from '../../trading/state/trading.state';
import { Owned } from '../../trading/models/owned-entity';
import { IOrder } from '../../trading/models/order';
import { IPosition } from '../../trading/models/position';

export type IPublisherOverviewViewModel = ITraderOverviewViewModel;

export class PublishersViewModelQueries {
  @Selector([DashboardPublishersState.publishers, TradingState.orders, TradingState.positions])
  static publishers(
    publishers: IPublisherOverview[],
    orders: Owned<IOrder>[],
    positions: Owned<IPosition>[]
  ): IPublisherOverviewViewModel[] {
    if (publishers.length === 0) {
      return [];
    }

    const publisher = publishers[0];

    const viewModel: IPublisherOverviewViewModel = {
      id: publisher.id,
      node: publisher.name,
      ping: publisher.ping,
      uptime: publisher.utcStartedAt ? Date.now() - publisher.utcStartedAt.getTime() : 0,
      formattedBalance:
        publisher.formattedBalance === 'No USDT wallet' ? '--' : publisher.formattedBalance,
      orders: orders.filter((it) => it.traderId === publisher.id).length,
      positions: positions.filter((it) => it.traderId === publisher.id).length,
      description: publisher.description,
      status: TraderStatus.stringify(publisher.status),
      // Fake data
      errors: Math.round(Math.random() > 0.55 ? (Math.random() % 0.2) * 10 : 0),
    };

    return [viewModel];
  }

  @Selector([PublishersViewModelQueries.publishers])
  static runningPublishers(
    publishers: IPublisherOverviewViewModel[]
  ): IPublisherOverviewViewModel[] {
    return filterTraders(publishers, TraderStatus.Running);
  }

  @Selector([PublishersViewModelQueries.publishers])
  static runningPublishersCount(publishers: IPublisherOverviewViewModel[]): number {
    return filterTraders(publishers, TraderStatus.Running).length;
  }

  @Selector([PublishersViewModelQueries.publishers])
  static pausedPublishersCount(publishers: IPublisherOverviewViewModel[]): number {
    return filterTraders(publishers, TraderStatus.Paused).length;
  }

  @Selector([PublishersViewModelQueries.publishers])
  static stoppedPublishersCount(publishers: IPublisherOverviewViewModel[]): number {
    return filterTraders(publishers, TraderStatus.Paused).length;
  }

  @Selector([PublishersViewModelQueries.publishers])
  static errorCount(publishers: IPublisherOverviewViewModel[]): number {
    return publishers.reduce((count, it) => count + it.errors, 0);
  }
}
