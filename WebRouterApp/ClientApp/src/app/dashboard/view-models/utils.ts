import { TraderStatus } from '../models/trader-status';

export function filterTraders<TTrader extends { status: string }>(
  traders: TTrader[],
  status: TraderStatus
): TTrader[] {
  return traders.filter((it) => it.status === TraderStatus.stringify(status));
}
