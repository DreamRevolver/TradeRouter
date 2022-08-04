import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output,
  ViewChild,
} from '@angular/core';
import { ISubscriberOverviewViewModel } from '../view-models/subscriber-overview.view-model-queries';
import { TraderStatus } from '../models/trader-status';
import { Guid } from '../../shared/types/guid';
import { TradersComponent } from './traders.component';

@Component({
  selector: 'app-subscribers',
  template: `
    <div class="column">
      <app-spinner [name]="'app-subscribers-spinner'" [showSpinner]="isBusy || false"></app-spinner>
      <div class="row left-justified">
        <button
          mat-stroked-button
          color="basic"
          [disabled]="!anySubscribers || !areAllSelectedStopped"
          (click)="emit(startRequested)"
        >
          <mat-icon>play_arrow</mat-icon>
          Start
        </button>
        <!--button
          mat-stroked-button
          color="basic"
          [disabled]="!anySubscribers || !areAllSelectedRunning"
          (click)="emit(pauseRequested)"
        >
          <mat-icon>pause</mat-icon>
          Pause
        </button-->
        <button
          mat-stroked-button
          color="basic"
          [disabled]="!anySubscribers || !areAllSelectedRunning"
          (click)="emit(stopRequested)"
        >
          <mat-icon>stop</mat-icon>
          Stop
        </button>
        <button
          mat-stroked-button
          color="basic"
          [disabled]="!anySubscribers || !areAllSelectedRunning"
          (click)="emit(syncRequested)"
        >
          <mat-icon>sync</mat-icon>
          Sync
        </button>
        <button
          mat-stroked-button
          color="basic"
          [disabled]="!anySubscribers || !areAllSelectedRunning"
          (click)="emit(closeAllPositionsRequested)"
        >
          <mat-icon>work_off</mat-icon>
          Close all Positions
        </button>
        <button
          mat-stroked-button
          color="basic"
          [disabled]="!anySubscribers || !areAllSelectedRunning"
          (click)="emit(closeAllOrdersRequested)"
        >
          <mat-icon>filter_list_off</mat-icon>
          Close all Orders
        </button>
        <button
          mat-stroked-button
          color="basic"
          [disabled]="!anySubscribers || !anySelected"
          (click)="requestDetails()"
        >
          <mat-icon>open_in_new</mat-icon>
          Details
        </button>
      </div>
      <app-traders
        [traders]="subscribers"
        [filterErrorsEnabled]="filterErrorsEnabled"
        [statusFilter]="statusFilter"
        [pagingEnabled]="true"
      >
        <ng-template #orders let-subscriber>
          {{ subscriber.publisherOrders }}/{{ subscriber.orders }}
        </ng-template>
        <ng-template #positions let-subscriber>
          {{ subscriber.publisherPositions }}/{{ subscriber.positions }}
        </ng-template>
      </app-traders>
    </div>
  `,
  styles: [
    `
      .mat-button-base {
        margin: 8px 8px 8px 0;
      }
    `,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SubscribersComponent {
  @Input() subscribers: ISubscriberOverviewViewModel[] | null = [];
  @Input() filterErrorsEnabled: boolean | null = false;
  @Input() statusFilter: TraderStatus | null = null;
  @Input() isBusy: boolean | null = false;

  @Output() startRequested = new EventEmitter<{ subscriberIds: Guid[] }>();
  @Output() pauseRequested = new EventEmitter<{ subscriberIds: Guid[] }>();
  @Output() stopRequested = new EventEmitter<{ subscriberIds: Guid[] }>();
  @Output() syncRequested = new EventEmitter<{ subscriberIds: Guid[] }>();
  @Output() closeAllOrdersRequested = new EventEmitter<{ subscriberIds: Guid[] }>();
  @Output() closeAllPositionsRequested = new EventEmitter<{ subscriberIds: Guid[] }>();
  @Output() detailsRequested = new EventEmitter<{ subscriberNames: string[] }>();

  @ViewChild(TradersComponent) grid: TradersComponent | null = null;

  public get anySubscribers(): boolean {
    return (this.subscribers?.length ?? 0) !== 0;
  }

  public get anySelected(): boolean {
    const selection = this.grid?.selection;
    return selection?.selected.length !== 0;
  }

  public get areAllSelectedRunning(): boolean {
    return this.areAllSelected(TraderStatus.Running);
  }

  public get areAllSelectedStopped(): boolean {
    return this.areAllSelected(TraderStatus.Stopped);
  }

  private areAllSelected(status: TraderStatus): boolean {
    const selection = this.grid?.selection;
    const selected =
      !selection?.selected || selection.selected.length === 0
        ? this.subscribers?.map((it) => it.id) ?? []
        : selection.selected;

    for (const id of selected) {
      const subscriber = this.subscribers?.find((it) => it.id === id);
      if (!subscriber || subscriber.status !== TraderStatus.stringify(status)) {
        return false;
      }
    }

    return true;
  }

  readonly emit = (eventEmitter: EventEmitter<{ subscriberIds: Guid[] }>): void => {
    const selected = this.grid?.selection.selected || [];
    const subscriberIds = [...selected];
    this.grid?.selection.clear();

    eventEmitter.emit({ subscriberIds });
  };

  readonly requestDetails = (): void => {
    const selected = this.grid?.selection.selected || [];
    const subscriberNames = (this.subscribers || [])
      .filter((it) => selected.includes(it.id))
      .map((it) => it.node);

    this.grid?.selection.clear();

    this.detailsRequested.emit({ subscriberNames });
  };
}
