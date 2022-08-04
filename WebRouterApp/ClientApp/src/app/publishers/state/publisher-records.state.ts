import { Injectable } from '@angular/core';
import { Action, Selector, State, StateContext } from '@ngxs/store';
import { IPublisherRecord } from '../models/publisher-record';
import { PublisherRecordsApiClient } from '../services/publisher-records.api-client';
import { PublisherRecordsApiActions } from './publisher-records.actions';
import produce from 'immer';
import { DashboardPublishersActions } from '../../dashboard/state/dashboard-publishers.actions';
import { hasActionsExecuting } from '@ngxs-labs/actions-executing';

export interface PublisherRecordsStateModel {
  defaults: Partial<PublisherRecordsStateModel>;
  publisherRecords: IPublisherRecord[];
  loaded: boolean;
}

const defaults = {
  publisherRecords: [],
  loaded: false,
};

@State<PublisherRecordsStateModel>({
  name: PublisherRecordsState.featureName,
  defaults: {
    ...defaults,
    defaults: defaults,
  },
})
@Injectable()
export class PublisherRecordsState {
  static featureName = 'publisherRecords';

  constructor(private publisherRecordsApi: PublisherRecordsApiClient) {}

  @Selector([
    hasActionsExecuting([PublisherRecordsApiActions.GetRecords, PublisherRecordsApiActions.Update]),
  ])
  static isBusy(hasActionsExecuting: boolean): boolean {
    return hasActionsExecuting;
  }

  @Selector()
  static publisherRecord(stateModel: PublisherRecordsStateModel): IPublisherRecord {
    return stateModel.publisherRecords[0];
  }

  @Action(PublisherRecordsApiActions.GetRecords)
  protected async getOverviews(context: StateContext<PublisherRecordsStateModel>): Promise<void> {
    if (context.getState().loaded) {
      return;
    }

    const records = await this.publisherRecordsApi.getRecords();

    context.patchState({
      publisherRecords: records,
      loaded: true,
    });
  }

  @Action(PublisherRecordsApiActions.Update)
  protected async update(
    context: StateContext<PublisherRecordsStateModel>,
    action: PublisherRecordsApiActions.Update
  ): Promise<void> {
    const update = action.payload.record;

    await this.publisherRecordsApi.update(update);

    const updatedRecord = { ...context.getState().publisherRecords[0], ...update };
    context.setState(
      produce((draft) => {
        draft.publisherRecords[0] = updatedRecord;
      })
    );

    await context
      .dispatch(new DashboardPublishersActions.RefreshOverview({ id: updatedRecord.id }))
      .toPromise();
  }
}
