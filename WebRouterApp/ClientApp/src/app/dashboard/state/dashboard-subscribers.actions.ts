import { ActionWith } from '../../shared/state/action-with-payload';
import { Guid } from '../../shared/types/guid';
import { TraderStatus } from '../models/trader-status';

export namespace DashboardSubscribersApiActions {
  const group = '[Dashboard Subscribers API]';

  export class GetOverviews extends ActionWith<void> {
    static type = `${group} Get Overviews`;
  }

  export class Start extends ActionWith<{ ids?: Guid[] }> {
    static type = `${group} Start`;
  }

  export class Pause extends ActionWith<{ ids?: Guid[] }> {
    static type = `${group} Pause`;
  }

  export class Stop extends ActionWith<{ ids?: Guid[] }> {
    static type = `${group} Stop`;
  }
}

export namespace DashboardSubscribersActions {
  const group = '[Dashboard Subscribers]';

  export class RefreshOverview extends ActionWith<{ id: Guid }> {
    static type = `${group} Refresh Overview`;
  }

  export class DeleteOverview extends ActionWith<{ id: Guid }> {
    static type = `${group} Delete Overview`;
  }

  export class SetFilterErrors extends ActionWith<{ enabled: boolean }> {
    static type = `${group} Set Filter Errors`;
  }

  export class ToggleFilterErrors extends ActionWith<void> {
    static type = `${group} Toggle Filter Errors`;
  }

  export class FilterByStatus extends ActionWith<{ status: TraderStatus | null }> {
    static type = `${group} Filter By Status`;
  }

  export class ViewTrades extends ActionWith<{ subscriberIds: Guid[]; subscriberNames: string[] }> {
    static type = `${group} View Trades`;
  }
}
