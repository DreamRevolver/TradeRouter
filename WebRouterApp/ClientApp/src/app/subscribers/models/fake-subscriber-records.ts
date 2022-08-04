import { CoeffKinds, ISubscriberRecord, TradeKinds } from './subscriber-record';
import { Guid } from '../../shared/types/guid';

export const SUBSCRIBER_RECORDS: ISubscriberRecord[] = [
  {
    id: Guid.from('DFE78E27-6E91-4873-B911-40C50FAB7E4D'),
    name: 'Subscriber First',
    description: 'This is the first subscriber',
    apiKey: 'a552521f2823e6583418462ff8e9d094ac6dda958bea017863f88d82908ea691',
    apiSecret: '6a6bd42fb8f3d956f6642962be712cc015d1736eebcb48cd2e974a7833a430af',
    multiplier: 0.5,
    coeffKind: CoeffKinds.CoeffToSize,
    tradeKind: TradeKinds.TradeAsMarket,
  },
  {
    id: Guid.from('A296D560-95BF-4066-BDA7-788740B55D42'),
    name: 'Subscriber Second',
    description: 'This is the second subscriber',
    apiKey: '3329ec7ae6bdffea7de1dd19a67a0b574f7a553c8e870bf9e062521e552b5615',
    apiSecret: 'eec08f26c98eb96268cc813afa159570c4441b490a3c72df85121b4baeab3a0b',
    coeffKind: CoeffKinds.CoeffToSize,
    multiplier: 0.7,
    tradeKind: TradeKinds.TradeAsMarket,
  },
];
