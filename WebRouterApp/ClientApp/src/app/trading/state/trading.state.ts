import { Injectable } from '@angular/core';
import { Action, NgxsOnInit, Selector, State, StateContext, Store } from '@ngxs/store';
import {
  TradingApiActions,
  TradingHubActions,
  TradingMessageReceivedActions,
} from './trading.actions';
import { Owned } from '../models/owned-entity';
import { IOrder } from '../models/order';
import { IPosition } from '../models/position';
import { TradingApiClient } from '../services/trading.api-client';
import { TradingHub } from '../services/trading-hub';
import { ScopedSubscription } from '../../shared/scoped-subscription/scoped-subscription';
import { assertPresent } from '../../shared/is-present';
import produce from 'immer';
import { AuthState } from '../../auth/state/auth.state';
import { HubConnectionState } from '@microsoft/signalr';
import { applyMessage } from './trading.state-mutators';
import { DashboardSubscribersActions } from '../../dashboard/state/dashboard-subscribers.actions';
import { Guid } from '../../shared/types/guid';

export interface TradingStateModel {
  defaults: Partial<TradingStateModel>;
  orders: Owned<IOrder>[];
  positions: Owned<IPosition>[];
  hubStarted: boolean;
  filterBySubscriberIds: Guid[];
  filterBySubscriberNames: string[];
}

const defaults = {
  orders: [],
  positions: [],
  hubStarted: false,
  filterBySubscriberIds: [],
  filterBySubscriberNames: [],
};

@State<TradingStateModel>({
  name: TradingState.featureName,
  defaults: {
    ...defaults,
    defaults: defaults,
  },
})
@Injectable()
export class TradingState implements NgxsOnInit {
  static featureName = 'trading';

  constructor(
    private tradingApi: TradingApiClient,
    private tradingHub: TradingHub,
    private ss: ScopedSubscription,
    private store: Store
  ) {
    this.ss
      .on(this.tradingHub.reconnected, () => {
        this.store.dispatch(new TradingHubActions.Reconnected());
      })
      .on(this.tradingHub.closed, (error) =>
        this.store.dispatch(new TradingHubActions.Closed(error))
      )
      .on(this.tradingHub.batchReceived, (batch) => {
        this.store.dispatch(new TradingMessageReceivedActions.Batch(batch));
      });
  }

  @Selector()
  static positions(stateModel: TradingStateModel): Owned<IPosition>[] {
    return stateModel.positions;
  }

  @Selector()
  static orders(stateModel: TradingStateModel): Owned<IOrder>[] {
    return stateModel.orders;
  }

  @Selector()
  static filterBySubscriberIds(stateModel: TradingStateModel): Guid[] {
    return stateModel.filterBySubscriberIds;
  }

  @Selector()
  static filterBySubscriberNames(stateModel: TradingStateModel): string[] {
    return stateModel.filterBySubscriberNames;
  }

  @Action(TradingApiActions.RequestTradingSnapshot)
  protected async requestTradingSnapshot(): Promise<void> {
    if (!this.tradingHub.connectionId) {
      return;
    }

    await this.tradingApi.requestTradingSnapshot(this.tradingHub.connectionId);
  }

  @Action(TradingApiActions.CancelAllOrdersOfSubscribers)
  protected async cancelAllOrdersOfSubscribers(
    context: StateContext<TradingStateModel>,
    action: TradingApiActions.CancelAllOrdersOfSubscribers
  ): Promise<void> {
    await this.tradingApi.cancelAllOrdersOfSubscribers(action.payload.ids);
    // We don't try to update the relevant orders' state here,
    // as we'll receive updates over the SignalR connection.
  }

  @Action(TradingApiActions.CloseAllPositionOfSubscribers)
  protected async closeAllPositionsOfSubscribers(
    context: StateContext<TradingStateModel>,
    action: TradingApiActions.CloseAllPositionOfSubscribers
  ): Promise<void> {
    await this.tradingApi.closeAllPositionsOfSubscribers(action.payload.ids);
    // We don't try to update the relevant orders' state here,
    // as we'll receive updates over the SignalR connection.
  }

  @Action(TradingApiActions.SyncOrdersOfSubscribers)
  protected async syncOrdersOfSubscribers(
    context: StateContext<TradingStateModel>,
    action: TradingApiActions.SyncOrdersOfSubscribers
  ): Promise<void> {
    await this.tradingApi.syncOrdersOfSubscribers(action.payload.ids);
    // We don't try to update the relevant orders' state here,
    // as we'll receive updates over the SignalR connection.
  }

  @Action(TradingApiActions.SyncPositionsOfSubscribers)
  protected async syncPositionsOfSubscribers(
    context: StateContext<TradingStateModel>,
    action: TradingApiActions.SyncPositionsOfSubscribers
  ): Promise<void> {
    await this.tradingApi.syncPositionsOfSubscribers(action.payload.ids);
    // We don't try to update the relevant orders' state here,
    // as we'll receive updates over the SignalR connection.
  }

  @Action(TradingHubActions.Start)
  protected async startTradingHub(context: StateContext<TradingStateModel>): Promise<void> {
    if (
      context.getState().hubStarted ||
      this.tradingHub.connectionState === HubConnectionState.Connecting
    ) {
      return;
    }

    await this.tradingHub.start();
    assertPresent(this.tradingHub.connectionId, 'this.tradingHub.connectionId');

    await this.tradingApi.requestTradingSnapshot(this.tradingHub.connectionId);

    context.patchState({
      hubStarted: true,
    });
  }

  @Action(TradingHubActions.Stop)
  protected async stopTradingHub(context: StateContext<TradingStateModel>): Promise<void> {
    await this.tradingHub.stop();

    context.patchState({
      orders: [],
      positions: [],
      hubStarted: false,
    });
  }

  @Action(TradingHubActions.Closed)
  protected async tradingHubClosed(
    context: StateContext<TradingStateModel>,
    action: TradingHubActions.Closed
  ): Promise<void> {
    if (!action.payload.error) {
      return;
    }

    console.error(action.payload.error);
  }

  @Action(TradingHubActions.Reconnected)
  protected async tradingHubReconnected(context: StateContext<TradingStateModel>): Promise<void> {
    if (!context.getState().hubStarted) {
      return;
    }

    assertPresent(this.tradingHub.connectionId, 'this.tradingHub.connectionId');
    await this.tradingApi.requestTradingSnapshot(this.tradingHub.connectionId);
  }

  @Action(TradingMessageReceivedActions.Batch)
  protected applyBatch(
    context: StateContext<TradingStateModel>,
    action: TradingMessageReceivedActions.Batch
  ): void {
    const batch = action.payload;
    context.setState(
      produce((draft) => {
        for (const message of batch.messages) {
          applyMessage(draft, message);
        }
      })
    );
  }

  @Action(DashboardSubscribersActions.ViewTrades)
  protected viewSubscriberTrades(
    context: StateContext<TradingStateModel>,
    action: DashboardSubscribersActions.ViewTrades
  ): void {
    context.patchState({
      filterBySubscriberIds: action.payload.subscriberIds,
      filterBySubscriberNames: action.payload.subscriberNames,
    });
  }

  async ngxsOnInit(context: StateContext<TradingStateModel>): Promise<void> {
    const isLoggedIn = this.store.selectSnapshot(AuthState.isLoggedIn);
    if (!isLoggedIn) {
      return;
    }

    await context.dispatch(new TradingHubActions.Start());
  }
}
