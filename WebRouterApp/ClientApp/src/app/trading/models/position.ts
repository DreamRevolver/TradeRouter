import { Owned } from './owned-entity';

export interface IPosition {
  symbol: string;
  entryPrice: number;
  leverage: number;
  positionAmt: number; // TODO: we either should rename it to size or we should rename size attribute for followers
  unRealizedProfit: number;
  positionSide: string;
}

export function equalPositions(left: Owned<IPosition>, right: Owned<IPosition>): boolean {
  return (
    left.traderId === right.traderId &&
    left.entity.symbol === right.entity.symbol &&
    left.entity.positionSide === right.entity.positionSide
  );
}

export function positionsContain(
  positions: Owned<IPosition>[],
  ownedPosition: Owned<IPosition>
): boolean {
  return positions.some((it) => equalPositions(it, ownedPosition));
}
