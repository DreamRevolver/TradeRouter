import { Injectable } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { CoeffKinds, ISubscriberRecord, TradeKinds } from '../models/subscriber-record';
import { SubscriberRecordsApiClient } from '../services/subscriber-records.api-client';
import {
  SubscriberRecordsActions,
  SubscriberRecordsApiActions,
} from './subscriber-records.actions';
import { SubscriberRecordDialogComponent } from '../components/subscriber-record-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import produce from 'immer';
import { DashboardSubscribersActions } from '../../dashboard/state/dashboard-subscribers.actions';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog.component';
import { assertDefined } from '../../shared/is-present';
import { hasActionsExecuting } from '@ngxs-labs/actions-executing';
import { isSubscriberRunningError } from '../models/api-errors';
import { isHttpErrorResponse } from '../../core/services/api/api.client';

export interface SubscriberRecordsStateModel {
  defaults: Partial<SubscriberRecordsStateModel>;
  subscriberRecords: ISubscriberRecord[];
  loaded: boolean;
  lastError: string | null;
}

const defaults = {
  subscriberRecords: [],
  loaded: false,
  lastError: null,
};

@State<SubscriberRecordsStateModel>({
  name: SubscriberRecordsState.featureName,
  defaults: {
    ...defaults,
    defaults: defaults,
  },
})
@Injectable()
export class SubscriberRecordsState {
  static featureName = 'subscriberRecords';

  constructor(
    private subscriberRecordsApi: SubscriberRecordsApiClient,
    private dialog: MatDialog
  ) {}

  @Selector()
  static subscriberRecords(stateModel: SubscriberRecordsStateModel): ISubscriberRecord[] {
    return stateModel.subscriberRecords;
  }

  private static readonly busyActions = [
    SubscriberRecordsActions.Add,
    SubscriberRecordsActions.Edit,
    SubscriberRecordsActions.Delete,
    SubscriberRecordsApiActions.GetRecords,
  ];

  @Selector([hasActionsExecuting(SubscriberRecordsState.busyActions)])
  static isBusy(hasActionsExecuting: boolean): boolean {
    return hasActionsExecuting;
  }

  @Selector()
  static lastError(stateModel: SubscriberRecordsStateModel): string | null {
    return stateModel.lastError;
  }

  @Action(SubscriberRecordsApiActions.GetRecords)
  protected async getOverviews(context: StateContext<SubscriberRecordsStateModel>): Promise<void> {
    if (context.getState().loaded) {
      return;
    }

    const records = await this.subscriberRecordsApi.getRecords();

    context.patchState({
      subscriberRecords: records,
      loaded: true,
    });
  }

  @Action(SubscriberRecordsActions.Add)
  protected async add(context: StateContext<SubscriberRecordsStateModel>): Promise<void> {
    const newRecord: Partial<ISubscriberRecord> = {
      name: 'New Subscriber',
      description: undefined,
      apiKey: undefined,
      apiSecret: undefined,
      coeffKind: CoeffKinds.CoeffToSize,
      multiplier: 0,
      tradeKind: TradeKinds.TradeAsMarket,
    };

    const dialogRef = this.dialog.open(SubscriberRecordDialogComponent, {
      width: '650px',
      data: newRecord,
    });

    const dialogResult: ISubscriberRecord | undefined = await dialogRef.afterClosed().toPromise();
    if (!dialogResult) {
      return;
    }

    await SubscriberRecordsState.try(async () => {
      const apiResult = await this.subscriberRecordsApi.add(dialogResult);

      newRecord.id = apiResult.id;

      context.setState(
        produce((draft) => {
          draft.subscriberRecords.push(dialogResult);
        })
      );

      await context
        .dispatch(new DashboardSubscribersActions.RefreshOverview({ id: newRecord.id }))
        .toPromise();
    }, context);
  }

  @Action(SubscriberRecordsActions.Edit)
  protected async edit(
    context: StateContext<SubscriberRecordsStateModel>,
    action: SubscriberRecordsActions.Edit
  ): Promise<void> {
    const record = action.payload.record;
    if (!record) {
      // TODO: Patch state with a descriptive message...
      return;
    }

    const dialogRef = this.dialog.open(SubscriberRecordDialogComponent, {
      width: '650px',
      data: { ...record }, //avoid 2-way binding
    });

    const dialogResult = await dialogRef.afterClosed().toPromise();
    if (!dialogResult) {
      return;
    }

    // record has more properties than dialogResult.
    const updatedRecord = { ...record, ...dialogResult };

    await SubscriberRecordsState.try(async () => {
      await this.subscriberRecordsApi.update(updatedRecord);

      context.setState(
        produce((draft) => {
          const i = draft.subscriberRecords.findIndex((it) => it.id === updatedRecord.id);
          draft.subscriberRecords[i] = updatedRecord;
        })
      );

      await context
        .dispatch(new DashboardSubscribersActions.RefreshOverview({ id: updatedRecord.id }))
        .toPromise();
    }, context);
  }

  @Action(SubscriberRecordsActions.Delete)
  protected async delete(
    context: StateContext<SubscriberRecordsStateModel>,
    action: SubscriberRecordsActions.Delete
  ): Promise<void> {
    const id = action.payload.id;
    if (!id) {
      // TODO: Patch state with a descriptive message...
      return;
    }

    const record = context.getState().subscriberRecords.find((it) => it.id === id);
    assertDefined(record);

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '650px',
      data: {
        title: 'Confirm deletion',
        message: `Delete '${record.name}' ('${record.description}')?`,
      },
    });

    const dialogResult = await dialogRef.afterClosed().toPromise();
    if (!dialogResult) {
      return;
    }

    await SubscriberRecordsState.try(async () => {
      await this.subscriberRecordsApi.delete(id);

      context.setState(
        produce((draft) => {
          draft.subscriberRecords = draft.subscriberRecords.filter((it) => it.id !== id);
        })
      );

      await context.dispatch(new DashboardSubscribersActions.DeleteOverview({ id })).toPromise();
    }, context);
  }

  @Action(SubscriberRecordsActions.DismissError)
  protected async dismissError(context: StateContext<SubscriberRecordsStateModel>): Promise<void> {
    context.patchState({ lastError: null });
  }

  private static async try(
    asyncAction: () => Promise<unknown>,
    context: StateContext<SubscriberRecordsStateModel>
  ): Promise<void> {
    try {
      await asyncAction();
      context.patchState({ lastError: null });
    } catch (error) {
      if (isHttpErrorResponse(error) && isSubscriberRunningError(error.error)) {
        context.patchState({ lastError: error.error.message });
        // We handle this error locally.
        // Don't let it bubble up to the global handler.
        return;
      }

      throw error;
    }
  }
}
