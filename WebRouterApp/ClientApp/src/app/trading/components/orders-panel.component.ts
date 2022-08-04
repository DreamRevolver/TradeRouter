import { SelectionModel } from '@angular/cdk/collections';
import {
  AfterViewInit,
  Component,
  Input,
  OnChanges,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { MatRow, MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { IPublisherOrderViewModel } from '../view-models/publisher-order.view-model';
import { ISubscriberOrderViewModel } from '../view-models/subscriber-order.view-model';
import { DataSourceFilterComponent } from '../../shared/components/datasource-filter.component';
import { ISubscriberPositionViewModel } from '../view-models/subscriber-position.view-model';

@Component({
  selector: 'app-orders-panel',
  template: `
    <mat-expansion-panel
      expanded="false"
      class="mat-elevation-z0"
      (opened)="arePublisherOrdersExpanded = true"
      (closed)="arePublisherOrdersExpanded = false"
    >
      <mat-expansion-panel-header>
        <mat-panel-title>
          <span class="section-header"> <i class="fa fa-brain"></i> Publisher </span>
        </mat-panel-title>
        <mat-panel-description>
          {{
            arePublisherOrdersExpanded
              ? 'Collapse to hide publisher orders'
              : 'Expand to show publisher orders'
          }}
        </mat-panel-description>
      </mat-expansion-panel-header>
      <table
        mat-table
        [dataSource]="dsPublisherOrders"
        [trackBy]="trackPublisherOrderBy"
        matSort
        matSortActive="unsynchronized"
        matSortDirection="desc"
        class="table-hover"
      >
        <!-- Checkbox Column -->
        <ng-container matColumnDef="select">
          <th mat-header-cell *matHeaderCellDef>
            <mat-checkbox></mat-checkbox>
          </th>
          <td mat-cell *matCellDef="let row">
            <mat-checkbox></mat-checkbox>
          </td>
        </ng-container>

        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Status</th>
          <td mat-cell *matCellDef="let element">{{ element.status }}</td>
        </ng-container>

        <ng-container matColumnDef="opentime">
          <th mat-header-cell *matHeaderCellDef>Time</th>
          <td mat-cell *matCellDef="let element">
            {{ element.opentime | date: 'yyyy/MM/dd HH:mm' }}
          </td>
        </ng-container>

        <!-- Node Column -->
        <ng-container matColumnDef="node">
          <th mat-header-cell *matHeaderCellDef>Node</th>
          <td mat-cell *matCellDef="let element">{{ element.node }}</td>
        </ng-container>

        <!-- Follows Column -->
        <ng-container matColumnDef="follows">
          <th mat-header-cell *matHeaderCellDef>Follows</th>
          <td mat-cell *matCellDef="let element">{{ element.follows }}</td>
        </ng-container>

        <!-- Symbol Column -->
        <ng-container matColumnDef="symbol">
          <th mat-header-cell *matHeaderCellDef>Symbol</th>
          <td mat-cell *matCellDef="let element">{{ element.symbol }}</td>
        </ng-container>

        <!-- Entry Price Column -->
        <ng-container matColumnDef="entryprice">
          <th mat-header-cell *matHeaderCellDef>Entry Price</th>
          <td mat-cell *matCellDef="let element">
            {{ element.entryprice | priceDecimalAuto }}
          </td>
        </ng-container>

        <!-- Symbol Column -->
        <ng-container matColumnDef="size">
          <th mat-header-cell *matHeaderCellDef>Size</th>
          <td mat-cell *matCellDef="let element">
            {{ element.size | priceDecimalAuto }}
          </td>
        </ng-container>
        <!-- Type Column -->
        <ng-container matColumnDef="type">
          <th mat-header-cell *matHeaderCellDef>Type</th>
          <td mat-cell *matCellDef="let element">{{ element.type }}</td>
        </ng-container>
        <!-- Side Column -->
        <ng-container matColumnDef="side">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Side</th>
          <td mat-cell *matCellDef="let element">{{ element.side }}</td>
        </ng-container>
        <!-- Syncronized Column -->
        <ng-container matColumnDef="unsynchronized">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Unsyncronized</th>
          <td
            mat-cell
            *matCellDef="let element"
            [ngStyle]="{ color: element.unsynchronized === 0 ? 'green' : 'red' }"
          >
            {{ element.unsynchronized === 0 ? 'All synced' : element.unsynchronized + ' orders' }}
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedPubColumns"></tr>
        <tr
          mat-row
          [ngClass]="{
            'not-synced': row.type === 'Not synced!',
            'manual-type': row.type === 'Manual'
          }"
          *matRowDef="let row; columns: displayedPubColumns"
          (click)="toggleSelectionOfSubscriberOrder(row)"
        ></tr>
      </table>
    </mat-expansion-panel>
    <mat-expansion-panel expanded class="mat-elevation-z0">
      <mat-expansion-panel-header>
        <mat-panel-title>
          <span class="section-header"><i class="fa fa-users"></i> Subscribers</span>
        </mat-panel-title>
      </mat-expansion-panel-header>
      <div class="row">
        <button mat-stroked-button>
          <mat-icon>sync</mat-icon>
          Force Sync
        </button>
        <button mat-stroked-button>
          <mat-icon>do_not_disturb_alt</mat-icon>
          Close Order
        </button>
        <button mat-stroked-button>
          <mat-icon>info</mat-icon>
          Details
        </button>
        <app-ds-filter #subscriberFilter [dataSource]="dsSubscriberOrders"></app-ds-filter>
      </div>
      <table
        mat-table
        [dataSource]="dsSubscriberOrders"
        [trackBy]="trackSubscriberOrderBy"
        matSort
        matSortActive="unsynchronized"
        matSortDirection="desc"
        class="table-hover"
      >
        <!-- Checkbox Column -->
        <ng-container matColumnDef="select">
          <th mat-header-cell *matHeaderCellDef>
            <mat-checkbox></mat-checkbox>
          </th>
          <td mat-cell *matCellDef="let row">
            <mat-checkbox></mat-checkbox>
          </td>
        </ng-container>

        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Status</th>
          <td mat-cell *matCellDef="let element">{{ element.status }}</td>
        </ng-container>

        <ng-container matColumnDef="opentime">
          <th mat-header-cell *matHeaderCellDef>Time</th>
          <td mat-cell *matCellDef="let element">
            {{ element.opentime | date: 'yyyy/MM/dd HH:mm' }}
          </td>
        </ng-container>

        <!-- Node Column -->
        <ng-container matColumnDef="node">
          <th mat-header-cell *matHeaderCellDef>Node</th>
          <td mat-cell *matCellDef="let element">{{ element.node }}</td>
        </ng-container>

        <!-- Follows Column -->
        <ng-container matColumnDef="follows">
          <th mat-header-cell *matHeaderCellDef>Follows</th>
          <td mat-cell *matCellDef="let element">{{ element.follows }}</td>
        </ng-container>

        <!-- Symbol Column -->
        <ng-container matColumnDef="symbol">
          <th mat-header-cell *matHeaderCellDef>Symbol</th>
          <td mat-cell *matCellDef="let element">{{ element.symbol }}</td>
        </ng-container>

        <!-- Entry Price Column -->
        <ng-container matColumnDef="entryprice">
          <th mat-header-cell *matHeaderCellDef>Entry Price</th>
          <td mat-cell *matCellDef="let element">
            {{ element.entryprice | priceDecimalAuto }}
          </td>
        </ng-container>

        <!-- Size Column -->
        <ng-container matColumnDef="size">
          <th mat-header-cell *matHeaderCellDef class="align-text-right">Size&nbsp;|</th>
          <td mat-cell *matCellDef="let element" class="align-text-right">
            {{ element.size | priceDecimalAuto }}&nbsp;|
          </td>
        </ng-container>
        <!-- Points Column -->
        <ng-container matColumnDef="points">
          <th mat-header-cell *matHeaderCellDef class="align-text-left">
            &nbsp;Points
            <mat-icon matTooltip="Size multiplied by the follower coeficient" inline>info</mat-icon>
          </th>
          <td mat-cell *matCellDef="let element" class="align-text-left">
            &nbsp;{{ element.points | priceDecimalAuto }}
            <span [ngStyle]="{ color: element.unsynchronized === 0 ? 'DarkGreen' : 'Firebrick' }">
              {{ element.diff | showDiff: element.size: element.side === 'SHORT'  }}
              <span *ngIf="element.unsynchronized !== 0">!</span>
            </span>
          </td>
        </ng-container>
        <!-- Type Column -->
        <ng-container matColumnDef="type">
          <th mat-header-cell *matHeaderCellDef>Type</th>
          <td mat-cell *matCellDef="let element">{{ element.type }}</td>
        </ng-container>
        <!-- Side Column -->
        <ng-container matColumnDef="side">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Side</th>
          <td mat-cell *matCellDef="let element">{{ element.side }}</td>
        </ng-container>
        <!-- Syncronized Column -->
        <ng-container matColumnDef="unsynchronized">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Unsyncronized</th>
          <td
            mat-cell
            *matCellDef="let element"
            [ngStyle]="{ color: element.unsynchronized === 0 ? 'green' : 'red' }"
          >
            {{ element.unsynchronized === 0 ? 'All synced' : element.unsynchronized + ' orders' }}
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedSubColumns"></tr>
        <tr
          mat-row
          [ngClass]="{
            'not-synced': row.type === 'Not synced!',
            'manual-type': row.type === 'Manual'
          }"
          *matRowDef="let row; columns: displayedSubColumns"
          (click)="selectedSubscriberOrders.toggle(row)"
        ></tr>
      </table>
      <mat-paginator [pageSize]="10"></mat-paginator>
    </mat-expansion-panel>
  `,
  styles: [
    `
      @import '../../../assets/styles/ftables.scss';
      @import '../../../assets/styles/filters.scss';
      @import '../../../assets/styles/panels.scss';

      table {
        width: 100%;
      }

      mat-paginator {
        background-color: transparent;
      }

      mat-card {
        margin-top: 10px;
        margin-bottom: 10px;
        margin-left: 10px;
      }
      .mat-header-row {
        height: 20px !important;
      }
      .mat-row {
        height: 20px !important;
      }

      .not-synced {
        background-color: lightpink;
      }

      .manual-type {
        background-color: rgb(209, 189, 13);
      }
    `,
  ],
})
export class OrdersPanelComponent implements AfterViewInit, OnChanges {
  @Input() publisherOrders: IPublisherOrderViewModel[] | null = [];
  @Input() subscriberOrders: ISubscriberOrderViewModel[] | null = [];
  @Input() filterBySubscriberNames: string[] | null = [];

  readonly displayedPubColumns = [
    'select',
    'opentime',
    'node',
    'symbol',
    'side',
    'entryprice',
    'size',
    //'unsynchronized',
  ];

  readonly displayedSubColumns = [
    'select',
    'opentime',
    'node',
    //'follows',
    'symbol',
    'side',
    'entryprice',
    'size',
    'points',
    'type',
    'status',
    //'unsynchronized',
  ];

  readonly dsPublisherOrders = new MatTableDataSource<IPublisherOrderViewModel>();

  readonly dsSubscriberOrders = new MatTableDataSource<ISubscriberOrderViewModel>();
  readonly selectedSubscriberOrders = new SelectionModel<string>(true, []);
  readonly subscribersFilter: string[] = ['Subscriber 31'];

  @ViewChild(MatSort) sortSubscriberOrders!: MatSort;
  @ViewChild(MatPaginator) paginatorSubscriberOrders!: MatPaginator;

  @ViewChild('subscriberFilter')
  subscriberFilter: DataSourceFilterComponent<ISubscriberPositionViewModel> | null = null;

  arePublisherOrdersExpanded = false;

  ngAfterViewInit(): void {
    this.dsSubscriberOrders.sort = this.sortSubscriberOrders;
    this.dsSubscriberOrders.paginator = this.paginatorSubscriberOrders;
    this.applyFilterBySubscriberNames();
  }

  ngOnChanges(changes: SimpleChanges): void {
    // console.log(`OrdersPanelComponent: ${changes}`);

    if (changes['publisherOrders']) {
      this.dsPublisherOrders.data = this.publisherOrders || [];
    }

    if (changes['subscriberOrders']) {
      this.dsSubscriberOrders.data = this.subscriberOrders || [];
    }

    if (changes['filterBySubscriberIds']) {
      this.applyFilterBySubscriberNames();
    }
  }

  private applyFilterBySubscriberNames() {
    for (const id of this.filterBySubscriberNames || []) {
      this.subscriberFilter?.addFilter('node', id);
    }
  }

  addSubscribersFilter(filter: string): void {
    const index = this.subscribersFilter.indexOf(filter);
    if (index < 0) {
      this.subscribersFilter.push(filter);
    }
  }

  removeSubscribersFilter(filter: string): void {
    const index = this.subscribersFilter.indexOf(filter);

    if (index >= 0) {
      this.subscribersFilter.splice(index, 1);
    }
    this.dsSubscriberOrders.filter = '';
  }

  // applySubscribersFilter(value: string): void {
  //   this.dsSubscribersOrders.filter = value.trim().toLowerCase();
  //
  //   if (this.dsSubscribersOrders.paginator) {
  //     this.dsSubscribersOrders.paginator.firstPage();
  //   }
  // }

  readonly clickPublisherRow = (row: MatRow): void => {
    console.log(row);
    // this.dsFollowers.filter = row.symbol;
    // this.addFilter('symbol:' + row.symbol);
  };

  readonly toggleSelectionOfSubscriberOrder = (row: ISubscriberOrderViewModel): void => {
    this.selectedSubscriberOrders.toggle(row.id);
  };

  readonly areAllSubscriberOrdersSelected = (): boolean => {
    const numSelected = this.selectedSubscriberOrders.selected.length;
    const numRows = this.dsSubscriberOrders.data.length;
    return numSelected === numRows;
  };

  readonly toggleAllSubscriberOrders = (): void => {
    if (this.areAllSubscriberOrdersSelected()) {
      this.selectedSubscriberOrders.clear();
      return;
    }

    this.selectedSubscriberOrders.select(...this.dsSubscriberOrders.data.map((it) => it.id));
  };

  readonly trackPublisherOrderBy = (index: number, row: IPublisherOrderViewModel): unknown =>
    row.id;

  readonly trackSubscriberOrderBy = (index: number, row: ISubscriberOrderViewModel): unknown =>
    row.id;
}
