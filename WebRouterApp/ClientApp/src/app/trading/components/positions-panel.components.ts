import { SelectionModel } from '@angular/cdk/collections';
import {
  AfterViewInit,
  Component,
  Input,
  OnChanges,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { IPublisherPositionViewModel } from '../view-models/publisher-position.view-model';
import { ISubscriberPositionViewModel } from '../view-models/subscriber-position.view-model';
import { DataSourceFilterComponent } from '../../shared/components/datasource-filter.component';

@Component({
  selector: 'app-positions-panel',
  template: `
    <mat-expansion-panel
      [expanded]="arePublisherPositionsExpanded"
      class="mat-elevation-z0"
      (opened)="arePublisherPositionsExpanded = true"
      (closed)="arePublisherPositionsExpanded = false"
    >
      <mat-expansion-panel-header>
        <mat-panel-title>
          <span class="section-header"> <i class="fa fa-brain"></i> Publisher </span>
        </mat-panel-title>
        <mat-panel-description>
          {{
            arePublisherPositionsExpanded
              ? 'Collapse to hide publisher positions'
              : 'Expand to show publisher positions'
          }}
        </mat-panel-description>
      </mat-expansion-panel-header>
      <table mat-table [dataSource]="dsPublisherPositions" [trackBy]="trackPublisherPositionBy">
        <ng-container matColumnDef="select">
          <th mat-header-cell *matHeaderCellDef>
            <mat-checkbox></mat-checkbox>
          </th>
          <td mat-cell *matCellDef="let row">
            <mat-checkbox></mat-checkbox>
          </td>
        </ng-container>

        <ng-container matColumnDef="opentime">
          <th mat-header-cell *matHeaderCellDef>Time</th>
          <td mat-cell *matCellDef="let element">
            {{ element.opentime | date: 'yyyy/MM/dd HH:mm' }}
          </td>
        </ng-container>

        <ng-container matColumnDef="node">
          <th mat-header-cell *matHeaderCellDef>Node</th>
          <td mat-cell *matCellDef="let element">{{ element.node }}</td>
        </ng-container>

        <ng-container matColumnDef="symbol">
          <th mat-header-cell *matHeaderCellDef>Symbol</th>
          <td mat-cell *matCellDef="let element">{{ element.symbol }}</td>
        </ng-container>

        <ng-container matColumnDef="entryprice">
          <th mat-header-cell *matHeaderCellDef>Entry Price</th>
          <td mat-cell *matCellDef="let element">
            {{ element.entryPrice.toFixed(4) }}
          </td>
        </ng-container>

        <ng-container matColumnDef="size">
          <th mat-header-cell *matHeaderCellDef>Size</th>
          <td mat-cell *matCellDef="let element">
            {{ element.size.toFixed(4) }}
          </td>
        </ng-container>

        <ng-container matColumnDef="upnl">
          <th mat-header-cell *matHeaderCellDef>Unrealized P/L</th>
          <td mat-cell *matCellDef="let element">
            {{ element.upnl.toFixed(4) }}
          </td>
        </ng-container>

        <ng-container matColumnDef="side">
          <th mat-header-cell *matHeaderCellDef>Side</th>
          <td mat-cell *matCellDef="let element">{{ element.side }}</td>
        </ng-container>

        <ng-container matColumnDef="unsynchronized">
          <th mat-header-cell *matHeaderCellDef>Syncronized</th>
          <td
            mat-cell
            *matCellDef="let element"
            [ngStyle]="{ color: element.unsynced === 0 ? 'green' : 'red' }"
          >
            {{ element.unsynced === 0 ? 'All synced' : element.unsynced + 'nodes are not in sync' }}
          </td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
      </table>
    </mat-expansion-panel>
    <mat-expansion-panel expanded class="mat-elevation-z0">
      <mat-expansion-panel-header>
        <mat-panel-title>
          <!--mat-icon inline>hearing</mat-icon-->
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
          Close Position
        </button>
        <button mat-stroked-button>
          <mat-icon>info</mat-icon>
          Details
        </button>
        <app-ds-filter #subscriberFilter [dataSource]="dsSubscriberPositions"></app-ds-filter>
      </div>
      <table
        mat-table
        [dataSource]="dsSubscriberPositions"
        [trackBy]="trackSubscriberPositionBy"
        matSort
        matSortActive="node"
        matSortDirection="asc"
      >
        <ng-container matColumnDef="select">
          <th mat-header-cell *matHeaderCellDef>
            <mat-checkbox></mat-checkbox>
          </th>
          <td mat-cell *matCellDef="let row">
            <mat-checkbox></mat-checkbox>
          </td>
        </ng-container>

        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Status</th>
          <td mat-cell *matCellDef="let element">
            <span style="visibility: hidden">{{ element.unsynchronized }}</span>
            <mat-icon inline [ngStyle]="{ color: element.unsynchronized === 0 ? 'green' : 'red' }">
              {{ element.unsynchronized > 0 ? 'sync_problem' : 'sync' }}</mat-icon
            >
          </td>
        </ng-container>

        <ng-container matColumnDef="opentime">
          <th mat-header-cell *matHeaderCellDef>Time</th>
          <td mat-cell *matCellDef="let element">
            {{ element.opentime | date: 'yyyy/MM/dd HH:mm' }}
          </td>
        </ng-container>

        <ng-container matColumnDef="node">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Node</th>
          <td mat-cell *matCellDef="let element">{{ element.node }}</td>
        </ng-container>

        <ng-container matColumnDef="follows">
          <th mat-header-cell *matHeaderCellDef>Follows</th>
          <td mat-cell *matCellDef="let element">{{ element.follows }}</td>
        </ng-container>

        <ng-container matColumnDef="symbol">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Symbol</th>
          <td mat-cell *matCellDef="let element">{{ element.symbol }}</td>
        </ng-container>

        <ng-container matColumnDef="entryprice">
          <th mat-header-cell *matHeaderCellDef>Entry Price / Diff</th>
          <td mat-cell *matCellDef="let element">
            {{ element.entryPrice | priceDecimalAuto }}
            <span
              [ngStyle]="{
                color:
                  element.entryPriceDelta * (element.side === 'LONG' ? -1 : 1) >= 0
                    ? 'DarkGreen'
                    : 'Firebrick'
              }"
            >
              {{ element.entryPriceDelta | showDiff: element.entryPrice: element.side === 'LONG' }}
            </span>
          </td>
        </ng-container>

        <ng-container matColumnDef="size">
          <th mat-header-cell *matHeaderCellDef class="align-text-right">Size&nbsp;|</th>
          <td mat-cell *matCellDef="let element" class="align-text-right">
            {{ element.size | priceDecimalAuto }}&nbsp;|
          </td>
        </ng-container>

        <ng-container matColumnDef="points">
          <th mat-header-cell *matHeaderCellDef class="align-text-left">
            &nbsp;Points
            <mat-icon matTooltip="Size multiplied by the subscriber coeficient" inline>info</mat-icon>
          </th>
          <td mat-cell *matCellDef="let element" class="align-text-left" matTooltip="{{element.size}} x {{(element.size / element.points).toFixed(2)}} = {{element.points}}">
            &nbsp;{{ element.points | priceDecimalAuto }}
            <span [ngStyle]="{ color: element.unsynchronized === 0 ? 'DarkGreen' : 'Firebrick' }">
              {{ element.diff | showDiff: element.size: element.side === 'SHORT' }}
              <span *ngIf="element.unsynchronized !== 0">!</span>
            </span>
          </td>
        </ng-container>

        <ng-container matColumnDef="upnl">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Unrealized P/L</th>
          <td mat-cell *matCellDef="let element">
            {{ element.upnl.toFixed(4) }}
          </td>
        </ng-container>

        <ng-container matColumnDef="side">
          <th mat-header-cell *matHeaderCellDef mat-sort-header>Side</th>
          <td mat-cell *matCellDef="let element">{{ element.side }}</td>
        </ng-container>

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
          *matRowDef="let row; columns: displayedSubColumns"
          (click)="selectionSubscriberPositions.toggle(row)"
        ></tr>
      </table>
      <mat-paginator [pageSize]="10" aria-label="Select page of users"></mat-paginator>
    </mat-expansion-panel>
  `,
  styles: [
    `
      @import '../../../assets/styles/ftables.scss';
      @import '../../../assets/styles/filters.scss';
      @import '../../../assets/styles/panels.scss';

      mat-paginator {
        background-color: transparent;
      }

      .mat-header-row {
        height: 20px !important;
      }
      .mat-row {
        height: 20px !important;
      }
    `,
  ],
})
// TODO: Create one common style for trading panel which will inherit all the table styles
export class PositionsPanelComponent implements AfterViewInit, OnChanges {
  @Input() publisherPositions: IPublisherPositionViewModel[] | null = [];
  @Input() subscriberPositions: ISubscriberPositionViewModel[] | null = [];
  @Input() filterBySubscriberNames: string[] | null = [];

  readonly displayedColumns = [
    'select',
    'opentime',
    'node',
    'symbol',
    'side',
    'entryprice',
    'size',
    'upnl',
    //'unsynchronized',
  ];

  readonly displayedSubColumns = [
    'select',
    'opentime',
    'node',
    // 'follows',
    'symbol',
    'side',
    'entryprice',
    'size',
    'points',
    'upnl',
    'status',
    // 'unsynchronized',
  ];

  readonly dsPublisherPositions = new MatTableDataSource<IPublisherPositionViewModel>();

  readonly dsSubscriberPositions = new MatTableDataSource<ISubscriberPositionViewModel>();
  readonly selectionSubscriberPositions = new SelectionModel<ISubscriberPositionViewModel>(
    true,
    []
  );

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  @ViewChild('subscriberFilter')
  subscriberFilter: DataSourceFilterComponent<ISubscriberPositionViewModel> | null = null;

  arePublisherPositionsExpanded = true;

  ngAfterViewInit(): void {
    this.dsSubscriberPositions.sort = this.sort;
    const defaultSortingDataAccessor = this.dsSubscriberPositions.sortingDataAccessor;
    this.dsSubscriberPositions.sortingDataAccessor = (row, columnName) => {
      const cellValue = defaultSortingDataAccessor(row, columnName);
      return cellValue + row.id;
    };
    this.dsSubscriberPositions.paginator = this.paginator;
    this.applyFilterBySubscriberNames();
  }

  ngOnChanges(changes: SimpleChanges): void {
    // console.log(`PositionsPanelComponent: ${changes}`);

    if (changes['publisherPositions']) {
      this.dsPublisherPositions.data = (this.publisherPositions || []).sort((a, b) =>
        a.id < b.id //
          ? -1
          : a.id > b.id
          ? 1
          : 0
      );
    }

    if (changes['subscriberPositions']) {
      this.dsSubscriberPositions.data = this.subscriberPositions || [];
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

  areAllSubscriberPositionsSelected(): boolean {
    const numSelected = this.selectionSubscriberPositions.selected.length;
    const numRows = this.dsSubscriberPositions.data.length;
    return numSelected === numRows;
  }

  readonly trackPublisherPositionBy = (index: number, row: IPublisherPositionViewModel): unknown =>
    row.id;

  readonly trackSubscriberPositionBy = (
    index: number,
    row: ISubscriberPositionViewModel
  ): unknown => row.id;
}
