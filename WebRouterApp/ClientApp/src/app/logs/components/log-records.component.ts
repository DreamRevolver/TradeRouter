import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  Input,
  OnChanges,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ILogRecordModel } from '../models/log-record.model';

@Component({
  selector: 'app-log-records',
  template: `
    <app-spinner [name]="'app-log-records-spinner'" [showSpinner]="isBusy || false"></app-spinner>
    <app-ds-filter [dataSource]="dataSource" [filters]="[]"></app-ds-filter>
    <table mat-table [dataSource]="dataSource">
      <app-column-template name="time">
        <ng-template #cell let-record>
          {{ record.unixSeconds | unixSecondsToDate | date: 'yyyy/MM/dd HH:mm:ss' }}
        </ng-template>
      </app-column-template>
      <app-column name="priority"></app-column>
      <app-column name="source"></app-column>
      <app-column name="message"></app-column>
      <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
      <tr mat-row *matRowDef="let record; columns: displayedColumns"></tr>
    </table>
    <mat-paginator
      *ngIf="pagingEnabled"
      [pageSize]="15"
      [pageSizeOptions]="[10, 15, 25, 100]"
      aria-label="Select page"
    ></mat-paginator>
  `,
  styles: [
    `
      table {
        width: 100%;
      }

      ::ng-deep .mat-column-time {
        white-space: nowrap;
        width: 15%;
      }
      ::ng-deep .mat-column-priority {
        width: 10%;
        padding-left: 8px !important;
      }
      ::ng-deep .mat-column-source {
        width: 20%;
        padding-left: 8px !important;
      }
      ::ng-deep .mat-column-message {
        width: 55%;
        padding-left: 8px !important;
      }
    `,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogRecordsComponent implements AfterViewInit, OnChanges {
  @Input() logRecords: ILogRecordModel[] | null = [];
  @Input() pagingEnabled = true;
  @Input() displayedColumns = ['time', 'priority', 'source', 'message'];
  @Input() isBusy: boolean | null = false;

  @ViewChild(MatPaginator) paginator: MatPaginator | null = null;
  @ViewChild(MatSort) sort: MatSort | null = null;

  readonly dataSource = new MatTableDataSource<ILogRecordModel>();

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['logRecords']) {
      this.dataSource.data = this.logRecords || [];
    }
  }

  ngAfterViewInit(): void {
    this.dataSource.data = this.logRecords || [];
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
}
