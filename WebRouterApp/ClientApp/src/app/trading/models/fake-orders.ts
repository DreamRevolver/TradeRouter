import { IPublisherOrderViewModel } from '../view-models/publisher-order.view-model';
import { ISubscriberOrderViewModel } from '../view-models/subscriber-order.view-model';
import { Guid } from '../../shared/types/guid';

const publisherId = Guid.from('95696A4D-0D23-438A-BBD2-0E3CE7C460EF');

const publisherOrders = [
  {
    id: `${publisherId}::web_M6CBdf9J0VokdoMFrHj2`,
    opentime: new Date(1632183564000),
    node: 'Master',
    symbol: 'BTCUSDT',
    side: 'Sell',
    status: 'NEW',
    client_Id: 'web_M6CBdf9J0VokdoMFrHj2',
    entryprice: 45702.3,
    size: 0.3,
    unsynchronized: 0,
  },
  {
    id: `${publisherId}::web_h78TtcIuDYSA5dn1vHNt`,
    opentime: new Date(1631420633000),
    node: 'Master',
    symbol: 'BTCUSDT',
    side: 'Sell',
    status: 'NEW',
    client_Id: 'web_h78TtcIuDYSA5dn1vHNt',
    entryprice: 43702.3,
    size: 0.194,
    unsynchronized: 0,
  },
  {
    id: `${publisherId}::web_7hwknrJw2nbX4rQH6GGp`,
    opentime: new Date(1630864308000),
    node: 'Master',
    symbol: 'BCHUSDT',
    side: 'Sell',
    status: 'NEW',
    client_Id: 'web_7hwknrJw2nbX4rQH6GGp',
    entryprice: 530.54,
    size: 78.955,
    unsynchronized: 0,
  },
  {
    id: `${publisherId}::web_Rwge4qIRJqoUBHYEYnnv`,
    opentime: new Date(1632336474000),
    node: 'Master',
    symbol: 'BCHUSDT',
    side: 'Sell',
    status: 'NEW',
    client_Id: 'web_Rwge4qIRJqoUBHYEYnnv',
    entryprice: 530.54,
    size: 18.953,
    unsynchronized: 0,
  },
  {
    id: `${publisherId}::web_9Vgyyu5jXaqtcZ14rgJg`,
    opentime: new Date(1632283985000),
    node: 'Master',
    symbol: 'ETHUSDT',
    side: 'Sell',
    status: 'NEW',
    client_Id: 'web_9Vgyyu5jXaqtcZ14rgJg',
    entryprice: 3201.73,
    size: 1,
    unsynchronized: 0,
  },
  {
    id: `${publisherId}::web_dXL442jHieWWAXD5RQ7Z`,
    opentime: new Date(1631449164000),
    node: 'Master',
    symbol: 'LTCUSDT',
    side: 'Buy',
    status: 'NEW',
    client_Id: 'web_dXL442jHieWWAXD5RQ7Z',
    entryprice: 132.48,
    size: 1,
    unsynchronized: 0,
  },
];

export const PUBLISHER_ORDERS: IPublisherOrderViewModel[] = publisherOrders;
export const SUBSCRIBERS_ORDERS: ISubscriberOrderViewModel[] = [];
let k = 0;
for (let i = 0; i < 67; i++) {
  for (let j = 0; j < PUBLISHER_ORDERS.length; j++) {
    const m = PUBLISHER_ORDERS[j];
    const coeff = Math.round(Math.random() * 10) / 10;
    const diff = (Math.random() / 50) * (Math.random() > 0.5 ? 1 : -1);
    const unsync = Math.random() > 0.99 ? 1 : 0;

    SUBSCRIBERS_ORDERS[k] = {
      id: `${Guid.generate()}::${m.client_Id}`,
      opentime: new Date(m.opentime.getTime() - Math.random() * 100),
      node: 'Subscriber ' + k,
      follows: m.node,
      status: m.status,
      client_Id: m.client_Id,
      symbol: m.symbol,
      mid: j,
      diff: diff,
      entryprice: m.entryprice,
      size: m.size * coeff,
      points: m.size,
      side: m.side,
      unsynchronized: unsync,
      type: unsync > 0 ? 'Not synced!' : Math.random() > 0.9 ? 'Manual' : 'Auto',
    };
    k++;
  }
}
