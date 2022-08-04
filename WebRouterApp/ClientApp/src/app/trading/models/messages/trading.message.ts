import { IPositionsSnapshotMessage } from './positions-snapshot.message';
import { IOrdersSnapshotMessage } from './orders-snapshot.message';
import { IOrdersChangedMessage } from './orders-changed.message';
import { IPositionsChangedMessage } from './positions-changed.message';

export type TradingMessage =
  | IPositionsSnapshotMessage
  | IPositionsChangedMessage
  | IOrdersSnapshotMessage
  | IOrdersChangedMessage;
