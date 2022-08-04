import { Guid } from '../../../shared/types/guid';
import { IPosition } from '../position';
import { TradingMessageTags } from './trading-message-tags';

export interface IPositionsChangedMessage {
  tag: TradingMessageTags.PositionsChanged;
  traderId: Guid;
  changeSet: IPositionsChangeSet;
}

export interface IPositionsChangeSet {
  added: IPosition[];
  updated: IPosition[];
  removed: IPosition[];
}
