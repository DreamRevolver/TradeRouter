import { Injectable } from '@angular/core';
import { TradeKinds } from '../models/subscriber-record';

@Injectable({ providedIn: 'root' })
export class TradeKindsRenderer {
  stringify(tradeKind: TradeKinds): string {
    switch (tradeKind) {
      case TradeKinds.TradeAsMarket:
        return 'Trade as market';
      default:
        return '--';
    }
  }
}
