import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { OrdersViewModelQueries } from '../view-models/orders.view-model-queries';
import { IPublisherOrderViewModel } from '../view-models/publisher-order.view-model';
import { ISubscriberOrderViewModel } from '../view-models/subscriber-order.view-model';
import { PositionsViewModelQueries } from '../view-models/positions.view-model-queries';
import { ISubscriberPositionViewModel } from '../view-models/subscriber-position.view-model';
import { IPublisherPositionViewModel } from '../view-models/publisher-position.view-model';
import { TradingApiActions } from '../state/trading.actions';
import { DashboardPublishersApiActions } from '../../dashboard/state/dashboard-publishers.actions';
import { DashboardSubscribersApiActions } from '../../dashboard/state/dashboard-subscribers.actions';
import { TradingState } from '../state/trading.state';

@Component({
  selector: 'app-trading-page',
  template: `
    <mat-card class="mat-elevation-z0">
      <mat-card-title>Positions</mat-card-title>
      <app-positions-panel
        [publisherPositions]="publisherPositions$ | async"
        [subscriberPositions]="subscriberPositions$ | async"
        [filterBySubscriberNames]="filterBySubscriberNames$ | async"
      ></app-positions-panel>
    </mat-card>
    <mat-card class="mat-elevation-z0">
      <mat-card-title>Orders</mat-card-title>
      <app-orders-panel
        [publisherOrders]="publisherOrders$ | async"
        [subscriberOrders]="subscriberOrders$ | async"
        [filterBySubscriberNames]="filterBySubscriberNames$ | async"
      ></app-orders-panel>
    </mat-card>
  `,
  styles: [
    `
      .section-header {
        font-size: large;
        margin-right: 40px;
      }

      mat-card {
        margin-bottom: 10px;
      }
    `,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TradingPageComponent implements OnInit {
  @Select(PositionsViewModelQueries.publisherPositions)
  readonly publisherPositions$!: Observable<IPublisherPositionViewModel[]>;

  @Select(PositionsViewModelQueries.subscriberPosition)
  readonly subscriberPositions$!: Observable<ISubscriberPositionViewModel[]>;

  @Select(OrdersViewModelQueries.publisherOrders)
  readonly publisherOrders$!: Observable<IPublisherOrderViewModel[]>;

  @Select(OrdersViewModelQueries.subscriberOrders)
  readonly subscriberOrders$!: Observable<ISubscriberOrderViewModel[]>;

  @Select(TradingState.filterBySubscriberNames)
  readonly filterBySubscriberNames$!: Observable<string[]>;

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.store.dispatch([
      new DashboardPublishersApiActions.GetOverviews(), //
      new DashboardSubscribersApiActions.GetOverviews(),
      new TradingApiActions.RequestTradingSnapshot(),
    ]);
  }
}
