import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { LogsActions } from '../state/logs.actions';
import { LogsState } from '../state/logs.state';
import { ILogRecordModel } from '../models/log-record.model';

@Component({
  selector: 'app-log-page',
  template: `
    <app-log-records
      [logRecords]="logRecords$ | async"
      [isBusy]="isBusy$ | async"
    ></app-log-records>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogPageComponent implements OnInit, OnDestroy {
  @Select(LogsState.logRecords)
  readonly logRecords$!: Observable<ILogRecordModel[]>;

  @Select(LogsState.isBusy)
  readonly isBusy$!: Observable<boolean>;

  constructor(private store: Store) {}

  ngOnInit(): void {
    this.store.dispatch([new LogsActions.StartPolling()]);
  }

  ngOnDestroy(): void {
    this.store.dispatch([new LogsActions.StopPolling()]);
  }
}
