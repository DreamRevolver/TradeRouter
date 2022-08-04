import { Guid } from '../../shared/types/guid';

export interface IPublisherPositionViewModel {
  id: string;
  opentime: Date;
  node: string;
  nodeId: Guid;
  symbol: string;
  entryPrice: number;
  leverage: number;
  size: number;
  upnl: number;
  side: string;
  unsynchronized: number;
}
