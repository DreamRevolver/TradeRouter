import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import {
  IPublisherOverviewViewModel,
  PublishersViewModelQueries,
} from '../view-models/publisher-overview.view-model-queries';
import { Observable } from 'rxjs';
import {
  ISubscriberOverviewViewModel,
  SubscriberOverviewViewModelQueries,
} from '../view-models/subscriber-overview.view-model-queries';
import { TraderStatus } from '../models/trader-status';
import { DashboardSubscribersState } from '../state/dashboard-subscribers.state';
import {
  DashboardPublishersActions,
  DashboardPublishersApiActions,
} from '../state/dashboard-publishers.actions';
import {
  DashboardSubscribersActions,
  DashboardSubscribersApiActions,
} from '../state/dashboard-subscribers.actions';
import { TradingApiActions } from '../../trading/state/trading.actions';
import { Guid } from '../../shared/types/guid';
import { DashboardPublishersState } from '../state/dashboard-publishers.state';

@Component({
  selector: 'app-dashboard-page',
  template: `
    <div class="column">
      <div class="row">
        <app-error-summary
          class="left-card"
          [errorsOnPublisher]="errorsOnPublisher$ | async"
          [errorsOnSubscribers]="errorsOnSubscribers$ | async"
          (filterErrorsRequested)="filterErrors()"
        >
        </app-error-summary>
        <app-status-summary
          class="right-card"
          [runningPublishersCount]="runningPublishersCount$ | async"
          [pausedPublishersCount]="pausedPublishersCount$ | async"
          [stoppedPublishersCount]="stoppedPublishersCount$ | async"
          [runningSubscribersCount]="runningSubscribersCount$ | async"
          [pausedSubscribersCount]="pausedSubscribersCount$ | async"
          [stoppedSubscribersCount]="stoppedSubscribersCount$ | async"
          (filterPublishersByStatusRequested)="filterPublishersByStatus($event)"
          (filterSubscribersByStatusRequested)="filterSubscribersByStatus($event)"
        ></app-status-summary>
      </div>

      <mat-card class="mat-elevation-z0">
        <mat-card-title>Publishers</mat-card-title>
        <mat-card-content>
          <div class="row">
            <div class="column">
              <app-publishers
                [publishers]="publishers$ | async"
                [statusFilter]="publishersStatusFilter$ | async"
                [filterErrorsEnabled]="publishersFilterErrorsEnabled$ | async"
              ></app-publishers>
            </div>
          </div>
        </mat-card-content>
      </mat-card>
      <mat-card class="mat-elevation-z0">
        <mat-card-title>Subscribers</mat-card-title>
        <mat-card-content>
          <div class="row">
            <div class="column">
              <app-subscribers
                [subscribers]="subscribers$ | async"
                [statusFilter]="subscribersStatusFilter$ | async"
                [filterErrorsEnabled]="subscribersFilterErrorsEnabled$ | async"
                [isBusy]="areSubscribersBusy$ | async"
                (startRequested)="startSubscribers($event)"
                (pauseRequested)="pauseSubscribers($event)"
                (stopRequested)="stopSubscribers($event)"
                (syncRequested)="syncSubscribers($event)"
                (closeAllOrdersRequested)="closeAllOrdersOfSubscribers($event)"
                (closeAllPositionsRequested)="closeAllPositionsOfSubscribers($event)"
                (detailsRequested)="requestDetailsOfSubscribers($event)"
              ></app-subscribers>
            </div>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [
    `
      mat-card {
        margin-bottom: 10px;
      }

      .left-card {
        width: 50%;
        margin-right: 5px;
        margin-bottom: 10px;
      }

      .right-card {
        width: 50%;
        margin-left: 5px;
        margin-bottom: 10px;
      }
    `,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DashboardPageComponent implements OnInit {
  @Select(PublishersViewModelQueries.publishers)
  readonly publishers$!: Observable<IPublisherOverviewViewModel[]>;

  @Select(SubscriberOverviewViewModelQueries.subscribers)
  readonly subscribers$!: Observable<ISubscriberOverviewViewModel[]>;

  @Select(DashboardPublishersState.filterErrorsEnabled)
  readonly publishersFilterErrorsEnabled$!: Observable<boolean>;

  @Select(DashboardSubscribersState.filterErrorsEnabled)
  readonly subscribersFilterErrorsEnabled$!: Observable<boolean>;

  @Select(DashboardPublishersState.statusFilter)
  readonly publishersStatusFilter$!: Observable<TraderStatus | null>;

  @Select(DashboardSubscribersState.statusFilter)
  readonly subscribersStatusFilter$!: Observable<TraderStatus | null>;

  @Select(PublishersViewModelQueries.errorCount)
  readonly errorsOnPublisher$!: Observable<number>;

  @Select(SubscriberOverviewViewModelQueries.errorCount)
  readonly errorsOnSubscribers$!: Observable<number>;

  @Select(PublishersViewModelQueries.runningPublishersCount)
  runningPublishersCount$!: Observable<number>;

  @Select(PublishersViewModelQueries.pausedPublishersCount)
  pausedPublishersCount$!: Observable<number>;

  @Select(PublishersViewModelQueries.stoppedPublishersCount)
  stoppedPublishersCount$!: Observable<number>;

  @Select(SubscriberOverviewViewModelQueries.runningSubscribersCount)
  runningSubscribersCount$!: Observable<number>;

  @Select(SubscriberOverviewViewModelQueries.pausedSubscribersCount)
  pausedSubscribersCount$!: Observable<number>;

  @Select(SubscriberOverviewViewModelQueries.stoppedSubscribersCount)
  stoppedSubscribersCount$!: Observable<number>;

  @Select(DashboardSubscribersState.isBusy)
  areSubscribersBusy$!: Observable<boolean>;

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.store.dispatch([
      new DashboardPublishersApiActions.GetOverviews(), //
      new DashboardSubscribersApiActions.GetOverviews(),
    ]);
  }

  filterErrors(): void {
    this.store.dispatch(new DashboardSubscribersActions.ToggleFilterErrors());
  }

  filterSubscribersByStatus(status: TraderStatus | null): void {
    this.store.dispatch(new DashboardSubscribersActions.FilterByStatus({ status }));
  }

  filterPublishersByStatus(status: TraderStatus | null): void {
    this.store.dispatch(new DashboardPublishersActions.FilterByStatus({ status }));
  }

  startSubscribers(arg: { subscriberIds: Guid[] }): void {
    this.store.dispatch(new DashboardSubscribersApiActions.Start({ ids: arg.subscriberIds }));
  }

  pauseSubscribers(arg: { subscriberIds: Guid[] }): void {
    console.log(`pauseSubscribers [${arg.subscriberIds}]`);
  }

  stopSubscribers(arg: { subscriberIds: Guid[] }): void {
    this.store.dispatch(new DashboardSubscribersApiActions.Stop({ ids: arg.subscriberIds }));
  }

  syncSubscribers(arg: { subscriberIds: Guid[] }): void {
    this.store.dispatch(
      new TradingApiActions.SyncPositionsOfSubscribers({ ids: arg.subscriberIds })
    );
  }

  closeAllOrdersOfSubscribers(arg: { subscriberIds: Guid[] }): void {
    this.store.dispatch(
      new TradingApiActions.CancelAllOrdersOfSubscribers({ ids: arg.subscriberIds })
    );
  }

  closeAllPositionsOfSubscribers(arg: { subscriberIds: Guid[] }): void {
    this.store.dispatch(
      new TradingApiActions.CloseAllPositionOfSubscribers({ ids: arg.subscriberIds })
    );
  }

  requestDetailsOfSubscribers(arg: { subscriberNames: string[] }): void {
    this.store.dispatch(
      new DashboardSubscribersActions.ViewTrades({
        subscriberIds: [],
        subscriberNames: arg.subscriberNames,
      })
    );
  }
}
