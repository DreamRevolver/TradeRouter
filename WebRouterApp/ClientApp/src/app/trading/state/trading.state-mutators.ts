import { Owned } from '../models/owned-entity';
import { IOrder, ordersContain } from '../models/order';
import { IPosition, positionsContain } from '../models/position';
import { Guid } from '../../shared/types/guid';
import { TradingMessageTags } from '../models/messages/trading-message-tags';
import { TradingMessage } from '../models/messages/trading.message';
import { WritableDraft } from 'immer/dist/types/types-external';
import { TradingStateModel } from './trading.state';

function makeOwn<T extends IOrder | IPosition>(
  traderId: Guid | undefined,
  entities: Readonly<T[]>
): Owned<T>[] {
  return entities.map((it) => ({ traderId, entity: it }));
}

export function applyMessage(
  draft: WritableDraft<Readonly<TradingStateModel>>,
  m: TradingMessage
): void {
  switch (m.tag) {
    case TradingMessageTags.PositionsSnapshot:
      draft.positions = [
        ...draft.positions.filter((it) => it.traderId !== m.traderId),
        ...makeOwn(m.traderId, m.positions),
      ];
      break;

    case TradingMessageTags.PositionsChanged: {
      const removed = makeOwn(m.traderId, m.changeSet.removed);
      const updated = makeOwn(m.traderId, m.changeSet.updated);
      const added = makeOwn(m.traderId, m.changeSet.added);

      const intactPositions = draft.positions.filter(
        (it) =>
          !positionsContain(removed, it) && //
          !positionsContain(updated, it)
      );

      draft.positions = [...intactPositions, ...updated, ...added];
      break;
    }

    case TradingMessageTags.OrdersSnapshot:
      draft.orders = [
        ...draft.orders.filter((it) => it.traderId !== m.traderId),
        ...makeOwn(m.traderId, m.orders),
      ];
      break;

    case TradingMessageTags.OrdersChanged: {
      const removed = makeOwn(m.traderId, m.changeSet.removed);
      const added = makeOwn(m.traderId, m.changeSet.added);
      const intactOrders = draft.orders.filter((it) => !ordersContain(removed, it));
      draft.orders = [...intactOrders, ...added];
      break;
    }
  }
}
