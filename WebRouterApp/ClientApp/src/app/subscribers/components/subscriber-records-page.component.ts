import { SelectionModel } from '@angular/cdk/collections';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatTable } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { ISubscriberRecord } from '../models/subscriber-record';
import { CoeffKindsRenderer } from '../services/coeff-kinds.renderer';
import { Select, Store } from '@ngxs/store';
import {
  SubscriberRecordsActions,
  SubscriberRecordsApiActions,
} from '../state/subscriber-records.actions';
import { SubscriberRecordsState } from '../state/subscriber-records.state';
import { Observable } from 'rxjs';
import { RxMatTableDataSource } from 'src/app/shared/material/table-data-source';
import { ScopedSubscription } from '../../shared/scoped-subscription/scoped-subscription';

@Component({
  selector: 'app-subscribers-records-page',
  template: `
    <div class="container">
      <app-spinner
        [name]="'app-subscribers-records-page-spinner'"
        [showSpinner]="(isBusy$ | async) || false"
      ></app-spinner>
      <div class="row">
        <button mat-stroked-button (click)="addSubscriber()">
          <mat-icon>add</mat-icon>
          Add
        </button>
        <button mat-stroked-button [disabled]="!isRecordSelected" (click)="editSubscriber()">
          <mat-icon>edit</mat-icon>
          Edit
        </button>
        <button mat-stroked-button [disabled]="!isRecordSelected" (click)="deleteSubscriber()">
          <mat-icon>delete</mat-icon>
          Delete
        </button>
        <button mat-stroked-button>
          <mat-icon>file_download</mat-icon>
          Export
        </button>
        <button mat-stroked-button>
          <mat-icon>file_upload</mat-icon>
          Import
        </button>
      </div>
      <app-error
        [showDismiss]="true"
        (dismissRequested)="dismissError()"
        [message]="lastError$ | async"
      ></app-error>
      <table mat-table [dataSource]="dataSource" class="table-hover">
        <ng-container matColumnDef="select">
          <th mat-header-cell *matHeaderCellDef>
            <mat-checkbox></mat-checkbox>
          </th>
          <td mat-cell *matCellDef="let row">
            <mat-checkbox></mat-checkbox>
          </td>
        </ng-container>

        <ng-container matColumnDef="name">
          <th mat-header-cell *matHeaderCellDef>Name</th>
          <td mat-cell *matCellDef="let row">{{ row.name }}</td>
        </ng-container>

        <ng-container matColumnDef="description">
          <th mat-header-cell *matHeaderCellDef>Description</th>
          <td mat-cell *matCellDef="let row">{{ row.description }}</td>
        </ng-container>

        <ng-container matColumnDef="apiKey">
          <th mat-header-cell *matHeaderCellDef>Api Key</th>
          <td mat-cell *matCellDef="let row">
            {{ row.apiKey.substr(0, 4) + '***********' + row.apiKey.substr(row.apiKey.length - 4) }}
          </td>
        </ng-container>

        <ng-container matColumnDef="apiSecret">
          <th mat-header-cell *matHeaderCellDef>Api Secret</th>
          <td mat-cell *matCellDef="let row">
            {{
              row.apiSecret.substr(0, 4) +
                '**********' +
                row.apiSecret.substr(row.apiSecret.length - 4)
            }}
          </td>
        </ng-container>

        <ng-container matColumnDef="coeffKind">
          <th mat-header-cell *matHeaderCellDef>Coefficient Type</th>
          <td mat-cell *matCellDef="let row">
            {{ this.coeffKindRenderer.stringify(row.coeffKind) }}
          </td>
        </ng-container>

        <ng-container matColumnDef="multiplier">
          <th mat-header-cell *matHeaderCellDef>Multiplier</th>
          <td mat-cell *matCellDef="let row">{{ row.multiplier.toFixed(2) }}</td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr
          mat-row
          [ngClass]="{ 'selected-cell': selection.isSelected(row) }"
          *matRowDef="let row; columns: displayedColumns"
          (click)="selection.clear(); selection.select(row)"
        ></tr>
      </table>
    </div>
  `,
  styles: [
    `
      @import '../../../assets/styles/ftables.scss';
      @import '../../../assets/styles/panels.scss';

      .selected-cell {
        background-color: #e6f2ff;
      }

      table {
        width: 100%;
      }
    `,
  ],
})
export class SubscriberRecordsPageComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = [
    'name',
    'description',
    'apiKey',
    'apiSecret',
    'coeffKind',
    'multiplier',
  ];

  @Select(SubscriberRecordsState.subscriberRecords)
  readonly subscriberRecords$!: Observable<ISubscriberRecord[]>;

  @Select(SubscriberRecordsState.isBusy)
  readonly isBusy$!: Observable<boolean>;

  @Select(SubscriberRecordsState.lastError)
  readonly lastError$!: Observable<string | null>;

  dataSource = new RxMatTableDataSource<ISubscriberRecord>(this.subscriberRecords$);
  selection = new SelectionModel<ISubscriberRecord>(false, []);

  @ViewChild(MatSort) sort: MatSort | null = null;
  @ViewChild(MatTable) table: MatTable<ISubscriberRecord> | null = null;

  constructor(
    public readonly coeffKindRenderer: CoeffKindsRenderer,
    private readonly store: Store,
    private readonly ss: ScopedSubscription
  ) {
    this.ss.on(this.subscriberRecords$, (subscriberRecords) => {
      const selected =
        this.selection.selected?.filter((s) => subscriberRecords.some((r) => r.id === s.id)) || [];

      if (this.selection.selected?.length === selected.length) {
        return;
      }

      this.selection.clear();
      this.selection.select(...selected);
    });
  }

  private get selectedRecord(): ISubscriberRecord | null {
    return this.selection.selected.length !== 0 ? this.selection.selected[0] : null;
  }

  public get isRecordSelected(): boolean {
    return this.selectedRecord !== null;
  }

  ngOnInit(): void {
    this.store.dispatch(new SubscriberRecordsApiActions.GetRecords());
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
  }

  addSubscriber(): void {
    this.store.dispatch(new SubscriberRecordsActions.Add());
  }

  editSubscriber(): void {
    this.store.dispatch(new SubscriberRecordsActions.Edit({ record: this.selectedRecord }));
  }

  deleteSubscriber(): void {
    this.store.dispatch(
      new SubscriberRecordsActions.Delete({ id: this.selectedRecord?.id ?? null })
    );
  }

  dismissError(): void {
    this.store.dispatch(new SubscriberRecordsActions.DismissError());
  }
}
