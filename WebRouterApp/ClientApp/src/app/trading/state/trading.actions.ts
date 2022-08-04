import { ActionWith } from '../../shared/state/action-with-payload';
import { IBatchMessage } from '../models/messages/batch.message';
import { Guid } from '../../shared/types/guid';

export namespace TradingApiActions {
  const group = '[Trading API]';

  export class RequestTradingSnapshot extends ActionWith<void> {
    static type = `${group} Request Trading Snapshot`;
  }

  export class CancelAllOrdersOfSubscribers extends ActionWith<{ ids?: Guid[] }> {
    static type = `${group} Cancel All Orders of Subscribers`;
  }

  export class CloseAllPositionOfSubscribers extends ActionWith<{ ids?: Guid[] }> {
    static type = `${group} Close All Positions of Subscribers`;
  }

  export class SyncOrdersOfSubscribers extends ActionWith<{ ids?: Guid[] }> {
    static type = `${group} Sync Orders of Subscribers`;
  }

  export class SyncPositionsOfSubscribers extends ActionWith<{ ids?: Guid[] }> {
    static type = `${group} Sync Positions of Subscribers`;
  }
}

export namespace TradingHubActions {
  const group = '[Trading Hub]';

  export class Start extends ActionWith<void> {
    static type = `${group} Start Trading Hub`;
  }
  export class Stop extends ActionWith<void> {
    static type = `${group} Stop Trading Hub`;
  }
  export class Reconnected extends ActionWith<void> {
    static type = `${group} Trading Hub Reconnected`;
  }
  export class Closed extends ActionWith<{ error: Error | null }> {
    static type = `${group} Trading Hub Closed`;
  }
}

export namespace TradingMessageReceivedActions {
  const group = '[Trading Message Received]';

  export class Batch extends ActionWith<IBatchMessage> {
    static type = `${group} Batch`;
  }
}
