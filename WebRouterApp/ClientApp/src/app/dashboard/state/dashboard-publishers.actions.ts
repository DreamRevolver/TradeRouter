import { Guid } from 'src/app/shared/types/guid';
import { ActionWith } from '../../shared/state/action-with-payload';
import { TraderStatus } from '../models/trader-status';

export namespace DashboardPublishersApiActions {
  const group = '[Dashboard Publishers API]';

  export class GetOverviews extends ActionWith<void> {
    static type = `${group} Get Overviews`;
  }
}

export namespace DashboardPublishersActions {
  const group = '[Dashboard Publishers]';

  export class RefreshOverview extends ActionWith<{ id: Guid }> {
    static type = `${group} Refresh Overview`;
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
}
