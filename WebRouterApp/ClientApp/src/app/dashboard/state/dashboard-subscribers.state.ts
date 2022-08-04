import { Injectable } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { TraderStatus } from '../models/trader-status';
import { SubscribersApiClient } from '../services/subscribers.api-client';
import { ISubscriberOverview } from '../models/subscriber-overview';
import {
  DashboardSubscribersActions,
  DashboardSubscribersApiActions,
} from './dashboard-subscribers.actions';
import { Guid } from '../../shared/types/guid';
import produce from 'immer';
import { WritableDraft } from 'immer/dist/types/types-external';
import { Router } from '@angular/router';
import { RoutePaths } from '../../core/constants/routes-paths';
import { hasActionsExecuting } from '@ngxs-labs/actions-executing';

export interface DashboardSubscribersStateModel {
  defaults: Partial<DashboardSubscribersStateModel>;
  subscribers: ISubscriberOverview[];
  loaded: boolean;
  filterErrorsEnabled: boolean;
  statusFilter: TraderStatus | null;
}

const defaults = {
  subscribers: [],
  loaded: false,
  filterErrorsEnabled: false,
  statusFilter: null,
};

@State<DashboardSubscribersStateModel>({
  name: DashboardSubscribersState.featureName,
  defaults: {
    ...defaults,
    defaults: defaults,
  },
})
@Injectable()
export class DashboardSubscribersState {
  static featureName = 'subscribers';

  constructor(
    private readonly subscribersApi: SubscribersApiClient,
    private readonly router: Router
  ) {}

  @Selector()
  static subscribers(stateModel: DashboardSubscribersStateModel): ISubscriberOverview[] {
    return stateModel.subscribers;
  }

  @Selector()
  static filterErrorsEnabled(stateModel: DashboardSubscribersStateModel): boolean {
    return stateModel.filterErrorsEnabled;
  }

  @Selector()
  static statusFilter(stateModel: DashboardSubscribersStateModel): TraderStatus | null {
    return stateModel.statusFilter;
  }

  private static readonly busyActions = [
    DashboardSubscribersApiActions.GetOverviews,
    DashboardSubscribersApiActions.Start,
    DashboardSubscribersApiActions.Pause,
    DashboardSubscribersApiActions.Stop,
    DashboardSubscribersActions.RefreshOverview,
    DashboardSubscribersActions.DeleteOverview,
  ];

  @Selector([hasActionsExecuting(DashboardSubscribersState.busyActions)])
  static isBusy(hasActionsExecuting: boolean): boolean {
    return hasActionsExecuting;
  }

  @Action(DashboardSubscribersApiActions.GetOverviews)
  protected async getOverviews(
    context: StateContext<DashboardSubscribersStateModel>
  ): Promise<void> {
    if (context.getState().loaded) {
      return;
    }

    const subscribers = await this.subscribersApi.getOverviews();

    context.patchState({
      subscribers: subscribers,
      loaded: true,
    });
  }

  @Action(DashboardSubscribersActions.RefreshOverview)
  protected async refreshOverview(
    context: StateContext<DashboardSubscribersStateModel>,
    action: DashboardSubscribersActions.RefreshOverview
  ): Promise<void> {
    const overviews = await this.subscribersApi.getOverviews([action.payload.id]);
    const overview = overviews[0];

    context.setState(
      produce((draft) => {
        const i = draft.subscribers.findIndex((it) => it.id === overview.id);
        if (i !== -1) {
          draft.subscribers[i] = overview;
        } else {
          draft.subscribers.push(overview);
        }
      })
    );
  }

  @Action(DashboardSubscribersActions.DeleteOverview)
  protected async deleteOverview(
    context: StateContext<DashboardSubscribersStateModel>,
    action: DashboardSubscribersActions.DeleteOverview
  ): Promise<void> {
    context.setState(
      produce((draft) => {
        draft.subscribers = draft.subscribers.filter((it) => it.id !== action.payload.id);
      })
    );
  }

  @Action(DashboardSubscribersApiActions.Start)
  protected async startSubscribers(
    context: StateContext<DashboardSubscribersStateModel>,
    action: DashboardSubscribersApiActions.Start
  ): Promise<void> {
    const response = await this.subscribersApi.start(action.payload.ids);

    const statusMap = new Map(response.statusOverviews.map((it) => [it.id, it.status]));
    context.setState(
      produce((draft) => {
        forEachSubscriber(
          draft.subscribers,
          action.payload.ids,
          (it) => (it.status = statusMap.get(it.id) ?? TraderStatus.Stopped)
        );
      })
    );
  }

  @Action(DashboardSubscribersApiActions.Pause)
  protected async pauseSubscribers(
    context: StateContext<DashboardSubscribersStateModel>,
    action: DashboardSubscribersApiActions.Pause
  ): Promise<void> {
    console.log(`DashboardSubscribersApiActions.Pause`);
  }

  @Action(DashboardSubscribersApiActions.Stop)
  protected async stopSubscribers(
    context: StateContext<DashboardSubscribersStateModel>,
    action: DashboardSubscribersApiActions.Stop
  ): Promise<void> {
    await this.subscribersApi.stop(action.payload.ids);

    context.setState(
      produce((draft) => {
        forEachSubscriber(
          draft.subscribers,
          action.payload.ids,
          (it) => (it.status = TraderStatus.Stopped)
        );
      })
    );
  }

  @Action(DashboardSubscribersActions.SetFilterErrors)
  protected setFilterErrors(
    context: StateContext<DashboardSubscribersStateModel>,
    action: DashboardSubscribersActions.SetFilterErrors
  ): void {
    if (action.payload.enabled === context.getState().filterErrorsEnabled) {
      return;
    }

    context.patchState({
      filterErrorsEnabled: action.payload.enabled,
    });
  }

  @Action(DashboardSubscribersActions.ToggleFilterErrors)
  protected toggleFilterErrors(context: StateContext<DashboardSubscribersStateModel>): void {
    context.patchState({
      filterErrorsEnabled: !context.getState().filterErrorsEnabled,
    });
  }

  @Action(DashboardSubscribersActions.FilterByStatus)
  protected filterByStatus(
    context: StateContext<DashboardSubscribersStateModel>,
    action: DashboardSubscribersActions.FilterByStatus
  ): void {
    context.patchState({
      statusFilter: action.payload.status,
    });
  }

  @Action(DashboardSubscribersActions.ViewTrades)
  protected async viewTrades(
    context: StateContext<DashboardSubscribersStateModel>,
    action: DashboardSubscribersActions.ViewTrades
  ): Promise<void> {
    await this.router.navigate([RoutePaths.trading]);
  }
}

function forEachSubscriber(
  subscribers: WritableDraft<ISubscriberOverview>[],
  ids: Guid[] | null | undefined,
  action: (it: WritableDraft<ISubscriberOverview>) => void
): void {
  ids = ids || [];
  for (const it of subscribers) {
    if (ids.length == 0 || ids.includes(it.id)) {
      action(it);
    }
  }
}
