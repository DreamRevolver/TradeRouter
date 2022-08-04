import { IPublisherRecord } from './publisher-record';
import { Guid } from '../../shared/types/guid';

export const fakePublisherRecord: IPublisherRecord = {
  id: Guid.from('567C8535-47D2-4E5D-98B3-9DC00CA67C22'),
  name: 'Publiser1',
  description: 'This publisher is the first one',
  apiKey: 'a552521f2823e6583418462ff8e9d094ac6dda958bea017863f88d82908ea691',
  apiSecret: '6a6bd42fb8f3d956f6642962be712cc015d1736eebcb48cd2e974a7833a430af',
  tradeAllOrdersAsMarket: false,
};
