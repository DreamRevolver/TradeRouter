import { Guid } from '../../shared/types/guid';
import { IPosition } from './position';
import { IOrder } from './order';

export interface Owned<T extends IOrder | IPosition> {
  entity: T;
  traderId?: Guid;
}

export function isOwnedByPublisher<T extends IOrder | IPosition>(ownedEntity: Owned<T>): boolean {
  return !ownedEntity.traderId;
}
