import { IPublisherPositionViewModel } from '../view-models/publisher-position.view-model';
import { ISubscriberPositionViewModel } from '../view-models/subscriber-position.view-model';
import { Guid } from '../../shared/types/guid';

const publisherId = Guid.from('95696A4D-0D23-438A-BBD2-0E3CE7C460EF');

const publisherPositions = [
  {
    id: `${publisherId}::LONG::LTCUSD`,
    opentime: new Date(1623445985000),
    node: 'Master',
    nodeId: Guid.empty,
    symbol: 'LTCUSD',
    entryPrice: 143.78,
    leverage: 2,
    liqprice: '',
    size: 1.5,
    points: 0,
    upnl: -0.53,
    side: 'LONG',
    unsynchronized: 0,
  },
  {
    id: `${publisherId}::SHORT::ETHUSD`,
    opentime: new Date(1613425985000),
    node: 'Master',
    nodeId: Guid.empty,
    symbol: 'ETHUSD',
    entryPrice: 2819.88,
    leverage: 2,
    liqprice: '8 985.00',
    size: 2.75,
    points: 0,
    upnl: 1.1,
    side: 'SHORT',
    unsynchronized: 3,
  },
  {
    id: `${publisherId}::LONG::BTCUSD`,
    opentime: new Date(1613445985000),
    node: 'Master',
    nodeId: Guid.empty,
    symbol: 'BTCUSD',
    entryPrice: 41117.23,
    leverage: 10,
    liqprice: '37 234.55',
    size: 2.358,
    points: 0,
    upnl: -175.74,
    side: 'LONG',
    unsynchronized: 2,
  },
];

export const PUBLISHER_POSITIONS: IPublisherPositionViewModel[] = publisherPositions;
export const SUBSCRIBERS_POSITIONS: ISubscriberPositionViewModel[] = [];
let k = 0;
for (let i = 0; i < 67; i++) {
  const subscriberId = Guid.generate();

  for (let j = 0; j < PUBLISHER_POSITIONS.length; j++) {
    const m = PUBLISHER_POSITIONS[j];
    const coeff = Math.round(Math.random() * 10) / 10;
    const diff = (Math.random() / 50) * (Math.random() > 0.5 ? 1 : -1);
    const unsynch =
      diff * (m.side === 'SHORT' ? -1 : 1) > 0.018 ? Math.round(Math.random() * 5) : 0;
    const entryprice = m.entryPrice + m.entryPrice * diff;

    SUBSCRIBERS_POSITIONS[k] = {
      id: `${subscriberId}::${m.side}::${m.symbol}`,
      opentime: new Date(m.opentime.getTime() - Math.random() * 100),
      node: 'Subscriber ' + i,
      nodeId: Guid.empty,
      follows: m.node,
      symbol: m.symbol,
      mid: j,
      diff: diff,
      entryPrice: entryprice,
      entryPriceDelta: entryprice - m.entryPrice,
      leverage: m.leverage,
      size: m.size + m.size * diff * coeff,
      points: m.size + m.size * diff,
      upnl: m.upnl,
      side: m.side,
      unsynchronized: unsynch,
    };
    k++;
  }
}
