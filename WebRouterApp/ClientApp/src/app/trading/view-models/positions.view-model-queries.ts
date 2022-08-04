import { Selector } from '@ngxs/store';
import { TradingState } from '../state/trading.state';
import { Owned } from '../models/owned-entity';
import { DashboardPublishersState } from '../../dashboard/state/dashboard-publishers.state';
import { IPublisherOverview } from '../../dashboard/models/publisher-overview';
import { IPublisherPositionViewModel } from './publisher-position.view-model';
import { ISubscriberPositionViewModel } from './subscriber-position.view-model';
import { IPosition } from '../models/position';
import { ISubscriberOverview } from '../../dashboard/models/subscriber-overview';
import { DashboardSubscribersState } from '../../dashboard/state/dashboard-subscribers.state';

import { Guid } from '../../shared/types/guid';

export class PositionsViewModelQueries {
  @Selector([DashboardPublishersState.publishers, TradingState.positions])
  static publisherPositions(
    publishers: IPublisherOverview[],
    positions: Owned<IPosition>[]
  ): IPublisherPositionViewModel[] {
    // return PUBLISHER_POSITIONS;

    if (publishers.length === 0) {
      return [];
    }

    const publisher = publishers[0]; // TODO: For multiple publishers we need to filter this by the corresponding publisher id

    const viewModels = positions
      .filter((it) => it.traderId === publisher.id)
      .map((it) => publisherPositionViewModelFrom(publisher, it));
    return viewModels;
  }

  @Selector([
    DashboardPublishersState.publishers,
    DashboardSubscribersState.subscribers,
    TradingState.positions,
  ])
  static subscriberPosition(
    publishers: IPublisherOverview[],
    subscribers: ISubscriberOverview[],
    positions: Owned<IPosition>[]
  ): ISubscriberPositionViewModel[] {
    // return SUBSCRIBERS_POSITIONS;

    if (publishers.length === 0) {
      return [];
    }

    const publisher = publishers[0];

    const viewModels = positions
      .filter((p) => p.traderId !== publisher.id)
      .map((p) =>
        subscriberPositionViewModelFrom(
          publisher,
          subscribers.find((s) => s.id === p.traderId) ?? null,
          positions,
          p
        )
      );
    return viewModels;
  }
}

function commonViewModelFrom(position: Owned<IPosition>) {
  const viewModel = {
    id: `${position.entity.positionSide}::${position.entity.symbol}::${position.traderId}`,
    opentime: new Date(), // TODO: Replace it with real date from the back-end ??
    symbol: position.entity.symbol,
    entryPrice: position.entity.entryPrice,
    leverage: position.entity.leverage,
    size: position.entity.positionAmt,
    upnl: position.entity.unRealizedProfit,
    side: position.entity.positionSide,
  };

  return viewModel;
}

function publisherPositionViewModelFrom(
  publisher: IPublisherOverview,
  position: Owned<IPosition>
): IPublisherPositionViewModel {
  const commonProps = commonViewModelFrom(position);
  const viewModel: IPublisherPositionViewModel = {
    ...commonProps,
    node: publisher.name,
    nodeId: publisher.id,
    unsynchronized: 2,
  };

  return viewModel;
}

function subscriberPositionViewModelFrom(
  publisher: IPublisherOverview,
  subscriber: ISubscriberOverview | null,
  positions: Owned<IPosition>[],
  position: Owned<IPosition>
): ISubscriberPositionViewModel {
  const commonProps = commonViewModelFrom(position);

  const pubPosition = positions.find(
    (it) =>
      it.traderId === publisher.id &&
      it.entity.symbol === position.entity.symbol &&
      it.entity.positionSide === position.entity.positionSide
  );
  const subMultiplier = subscriber?.multiplier ?? 1;
  const subPoints = commonProps.size / subMultiplier;
  const pubSize = pubPosition?.entity.positionAmt ?? commonProps.size * subMultiplier;

  const diff = Math.abs(pubSize - subPoints);

  // TODO: This is a threshold in % which defines what position difference will be considered as unsynced.
  //       This must become configurable
  const UNSYNC_THRESHOLD = 0.018;
  const unsynchronized = diff / Math.abs(commonProps.size) > UNSYNC_THRESHOLD ? 1 : 0;

  const viewModel: ISubscriberPositionViewModel = {
    ...commonProps,
    node: subscriber?.name ?? '???',
    nodeId: subscriber?.id ?? Guid.empty,
    follows: publisher.name,
    entryPriceDelta: pubPosition ? commonProps.entryPrice - pubPosition.entity.entryPrice : 0,
    mid: 1,
    diff,
    entryPrice: commonProps.entryPrice,
    size: commonProps.size,
    points: subPoints,
    unsynchronized,
  };

  return viewModel;
}
