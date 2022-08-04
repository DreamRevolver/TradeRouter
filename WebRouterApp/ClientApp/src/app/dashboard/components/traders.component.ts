import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ContentChild,
  Input,
  OnChanges,
  SimpleChanges,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableDataSource } from '@angular/material/table';
import { TraderStatusRenderer } from '../services/trader-status.renderer';
import { TraderStatus } from '../models/trader-status';
import { ITraderOverviewViewModel } from '../view-models/trader-overview.view-model';
import { Guid } from '../../shared/types/guid';
import { isPresent } from '../../shared/is-present';

@Component({
  selector: 'app-traders',
  template: `
    <div class="column">
      <table mat-table [dataSource]="dataSource" [trackBy]="trackBy">
        <app-column-checkbox
          name="select"
          [anySelected]="selection.hasValue()"
          [allSelected]="areAllSelected()"
          [isSelected]="isSelected"
          (toggleRequested)="toggle($event)"
          (toggleAllRequested)="toggleAll()"
        ></app-column-checkbox>

        <app-column name="node"></app-column>
        <app-column name="formattedBalance" label="Balance"></app-column>

        <app-column-template name="uptime">
          <ng-template #cell let-trader>{{ trader.uptime | timeSpan }}</ng-template>
        </app-column-template>

        <app-column-template name="orders">
          <ng-template #cell let-trader>
            <ng-container
              *ngTemplateOutlet="ordersTemplate; context: { $implicit: trader }"
            ></ng-container>
          </ng-template>
        </app-column-template>

        <app-column-template name="positions">
          <ng-template #cell let-trader>
            <ng-container
              *ngTemplateOutlet="positionsTemplate; context: { $implicit: trader }"
            ></ng-container>
          </ng-template>
        </app-column-template>

        <app-column name="description"> </app-column>
        <app-column name="errors"></app-column>

        <app-column-template name="status">
          <ng-template #cell let-trader>
            <span [ngStyle]="{ color: statusRenderer.colorFor(trader.status) }">{{
              trader.status
            }}</span>
          </ng-template>
        </app-column-template>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr
          mat-row
          *matRowDef="let trader; columns: displayedColumns"
          (click)="toggle(trader)"
        ></tr>
      </table>
    </div>
    <mat-paginator *ngIf="pagingEnabled" [pageSize]="16" aria-label="Select page"></mat-paginator>
  `,
  styles: [
    `
      table {
        width: 100%;
      }

      mat-paginator {
        background-color: transparent;
      }

      .mat-button-base {
        margin: 8px 8px 8px 0;
      }
    `,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TradersComponent implements AfterViewInit, OnChanges {
  @Input() traders: ITraderOverviewViewModel[] | null = [];
  @Input() filterErrorsEnabled: boolean | null = false;
  @Input() statusFilter: TraderStatus | null = null;

  @Input() pagingEnabled = false;
  @Input() displayedColumns = [
    'select',
    'node',
    'formattedBalance',
    'status',
    'uptime',
    'orders',
    'positions',
    'description',
    // 'errors',
  ];

  constructor(public statusRenderer: TraderStatusRenderer) {}

  @ContentChild('orders', { static: false })
  ordersTemplate: TemplateRef<unknown> | null = null;

  @ContentChild('positions', { static: false })
  positionsTemplate: TemplateRef<unknown> | null = null;

  readonly dataSource = new MatTableDataSource<ITraderOverviewViewModel>();
  readonly selection = new SelectionModel<Guid>(true, []);

  @ViewChild(MatPaginator) paginator: MatPaginator | null = null;
  @ViewChild(MatSort) sort: MatSort | null = null;

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  ngOnChanges(changes: SimpleChanges): void {
    // console.log(`TradersComponent: ${changes}`);

    if (changes['traders']) {
      this.dataSource.data = this.traders || [];
    }

    if (changes['filterErrorsEnabled']) {
      if (this.filterErrorsEnabled) {
        this.dataSource.filterPredicate = (it, filter) => it.errors > parseInt(filter);
        this.dataSource.filter = '0';
      } else {
        this.dataSource.filterPredicate = () => true;
        this.dataSource.filter = '';
      }
    }

    if (changes['statusFilter']) {
      if (this.statusFilter !== null) {
        this.dataSource.filterPredicate = (it, filter) => it.status === filter;
        this.dataSource.filter = isPresent(this.statusFilter)
          ? TraderStatus.stringify(this.statusFilter)
          : '';
      } else {
        this.dataSource.filterPredicate = () => true;
        this.dataSource.filter = '';
      }
    }
  }

  readonly isSelected = (trader: ITraderOverviewViewModel): boolean =>
    this.selection.isSelected(trader.id);

  readonly toggle = (trader: ITraderOverviewViewModel): void => {
    this.selection.toggle(trader.id);
  };

  readonly areAllSelected = (): boolean => {
    const numSelected = this.selection.selected.length;
    if (numSelected === 0) {
      return false;
    }

    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  };

  readonly toggleAll = (): void => {
    if (this.areAllSelected()) {
      this.selection.clear();
      return;
    }

    this.selection.select(...this.dataSource.data.map((it) => it.id));
  };

  readonly trackBy = (index: number, row: ITraderOverviewViewModel): unknown => row.id;
}
