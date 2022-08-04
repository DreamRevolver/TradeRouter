import { Injectable } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import {
  DashboardPublishersActions,
  DashboardPublishersApiActions,
} from './dashboard-publishers.actions';
import { IPublisherOverview } from '../models/publisher-overview';
import { TraderStatus } from '../models/trader-status';
import { PublishersApiClient } from '../services/publishers.api-client';
import produce from 'immer';

export interface DashboardPublishersStateModel {
  defaults: Partial<DashboardPublishersStateModel>;
  publishers: IPublisherOverview[];
  loaded: boolean;
  filterErrorsEnabled: boolean;
  statusFilter: TraderStatus | null;
}

const defaults = {
  publishers: [],
  loaded: false,
  filterErrorsEnabled: false,
  statusFilter: null,
};

@State<DashboardPublishersStateModel>({
  name: DashboardPublishersState.featureName,
  defaults: {
    ...defaults,
    defaults: defaults,
  },
})
@Injectable()
export class DashboardPublishersState {
  static featureName = 'publishers';

  constructor(private publishersApi: PublishersApiClient) {}

  @Selector()
  static publishers(stateModel: DashboardPublishersStateModel): IPublisherOverview[] {
    return stateModel.publishers;
  }

  @Selector()
  static filterErrorsEnabled(stateModel: DashboardPublishersStateModel): boolean {
    return stateModel.filterErrorsEnabled;
  }

  @Selector()
  static statusFilter(stateModel: DashboardPublishersStateModel): TraderStatus | null {
    return stateModel.statusFilter;
  }

  @Action(DashboardPublishersActions.RefreshOverview)
  protected async refreshOverview(
    context: StateContext<DashboardPublishersStateModel>,
    action: DashboardPublishersActions.RefreshOverview
  ): Promise<void> {
    const overviews = await this.publishersApi.getOverviews([action.payload.id]);
    const overview = overviews[0];

    context.setState(
      produce((draft) => {
        const i = draft.publishers.findIndex((it) => it.id === overview.id);
        if (i !== -1) {
          draft.publishers[i] = overview;
        } else {
          draft.publishers.push(overview);
        }
      })
    );
  }

  @Action(DashboardPublishersApiActions.GetOverviews)
  protected async getOverviews(
    context: StateContext<DashboardPublishersStateModel>
  ): Promise<void> {
    if (context.getState().loaded) {
      return;
    }

    const publishers = await this.publishersApi.getOverviews();

    context.patchState({
      publishers: publishers,
      loaded: true,
    });
  }

  @Action(DashboardPublishersActions.SetFilterErrors)
  protected setFilterErrors(
    context: StateContext<DashboardPublishersStateModel>,
    action: DashboardPublishersActions.SetFilterErrors
  ): void {
    if (action.payload.enabled === context.getState().filterErrorsEnabled) {
      return;
    }

    context.patchState({
      filterErrorsEnabled: action.payload.enabled,
    });
  }

  @Action(DashboardPublishersActions.ToggleFilterErrors)
  protected toggleFilterErrors(context: StateContext<DashboardPublishersStateModel>): void {
    context.patchState({
      filterErrorsEnabled: !context.getState().filterErrorsEnabled,
    });
  }

  @Action(DashboardPublishersActions.FilterByStatus)
  protected filterByStatus(
    context: StateContext<DashboardPublishersStateModel>,
    action: DashboardPublishersActions.FilterByStatus
  ): void {
    context.patchState({
      statusFilter: action.payload.status,
    });
  }
}
