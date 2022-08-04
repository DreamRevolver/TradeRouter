import { Injectable } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { ILogRecordModel } from '../models/log-record.model';
import { LogsActions, LogsApiActions } from './logs.actions';
import { LogsApiClient } from '../services/logs.api-client';
import { interval } from 'rxjs';
import { ScopedSubscription } from '../../shared/scoped-subscription/scoped-subscription';

export interface LogsStateModel {
  defaults: Partial<LogsStateModel>;
  logs: ILogRecordModel[];
  pollingStarted: boolean;
}

const defaults = {
  logs: [],
  pollingStarted: false,
};

@State<LogsStateModel>({
  name: LogsState.featureName,
  defaults: {
    ...defaults,
    defaults: defaults,
  },
})
@Injectable()
export class LogsState {
  constructor(private logsApi: LogsApiClient, private ss: ScopedSubscription) {}

  static featureName = 'logs';

  @Selector()
  static logRecords(stateModel: LogsStateModel): ILogRecordModel[] {
    return stateModel.logs;
  }

  @Selector()
  static isBusy(stateModel: LogsStateModel): boolean {
    return !stateModel.pollingStarted;
  }

  @Action(LogsApiActions.GetAll)
  protected async getAll(context: StateContext<LogsStateModel>): Promise<void> {
    context.patchState({
      logs: await this.logsApi.getAll(),
    });
  }

  @Action(LogsActions.StartPolling)
  protected async startPolling(context: StateContext<LogsStateModel>): Promise<void> {
    context.patchState({
      logs: await this.logsApi.getAll(),
    });
    context.patchState({
      pollingStarted: true,
    });
  }

  @Action(LogsActions.StopPolling)
  protected async stopPolling(context: StateContext<LogsStateModel>): Promise<void> {
    context.patchState({
      pollingStarted: false,
    });
  }

  ngxsOnInit(context: StateContext<LogsStateModel>): void {
    this.ss.on(interval(1000), () => {
      if (context.getState().pollingStarted) {
        context.dispatch(new LogsApiActions.GetAll());
      }
    });
  }
}
