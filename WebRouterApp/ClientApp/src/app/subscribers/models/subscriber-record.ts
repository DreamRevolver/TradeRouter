import { Guid } from '../../shared/types/guid';

export interface ISubscriberRecord {
  id: Guid;
  apiKey: string;
  apiSecret: string;
  name: string;
  description: string;
  multiplier: number;
  coeffKind: CoeffKinds;
  tradeKind: TradeKinds;
}

export enum TradeKinds {
  TradeAsMarket = 0,
}

export namespace TradeKinds {
  export function stringify(tradeKind: TradeKinds): string {
    switch (tradeKind) {
      case TradeKinds.TradeAsMarket:
        return 'Trade as market';
      default:
        return '--';
    }
  }
}

export enum CoeffKinds {
  CoeffToSize = 0,
}

export namespace CoeffKinds {
  export function stringify(coeffKind: CoeffKinds): string {
    switch (coeffKind) {
      case CoeffKinds.CoeffToSize:
        return 'Coefficient to size';
      default:
        return '--';
    }
  }
}
