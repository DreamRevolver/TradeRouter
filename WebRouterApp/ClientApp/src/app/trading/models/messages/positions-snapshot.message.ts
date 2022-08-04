import { Guid } from '../../../shared/types/guid';
import { IPosition } from '../position';
import { TradingMessageTags } from './trading-message-tags';

export interface IPositionsSnapshotMessage {
  tag: TradingMessageTags.PositionsSnapshot;
  traderId: Guid;
  positions: IPosition[];
}
