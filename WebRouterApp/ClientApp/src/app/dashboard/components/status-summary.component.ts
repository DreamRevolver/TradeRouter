import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TraderStatus } from '../models/trader-status';

@Component({
  selector: 'app-status-summary',
  template: `
    <mat-card class = "mat-elevation-z0">
      <mat-card-subtitle>Status summary</mat-card-subtitle>
      <mat-card-content>
        <i class="fa fa-diamond"></i>
        <span class="link" (click)="filterPublishersByStatusRequested.emit(null)">Publisher nodes: </span>
        <span
          style="color: green"
          class="link"
          (click)="filterPublishersByStatusRequested.emit(TraderStatus.Running)"
        >
          <i class="fa fa-play-circle"></i> {{ runningPublishersCount }} running</span
        >,
        <span
          style="color: grey"
          class="link"
          (click)="filterPublishersByStatusRequested.emit(TraderStatus.Paused)"
          ><i class="fa fa-pause-circle"></i> {{ pausedPublishersCount }} paused</span
        >,
        <span
          style="color: blue"
          class="link"
          (click)="filterPublishersByStatusRequested.emit(TraderStatus.Stopped)"
          ><i class="fa fa-stop-circle"></i> {{ stoppedPublishersCount }} stopped.
        </span>
        <br />
        <i class="fa fa-code-fork"></i>
        <span class="link" (click)="filterSubscribersByStatusRequested.emit(null)">Subscriber nodes: </span>
        <span
          style="color: green"
          class="link"
          (click)="filterSubscribersByStatusRequested.emit(TraderStatus.Running)"
          ><i class="fa fa-play-circle"></i> {{ runningSubscribersCount }} running</span
        >,
        <span
          style="color: grey"
          class="link"
          (click)="filterSubscribersByStatusRequested.emit(TraderStatus.Paused)"
          ><i class="fa fa-pause-circle"></i> {{ pausedSubscribersCount }} paused</span
        >,
        <span
          style="color: blue"
          class="link"
          (click)="filterSubscribersByStatusRequested.emit(TraderStatus.Stopped)"
          ><i class="fa fa-stop-circle"></i> {{ stoppedSubscribersCount }} stopped.
        </span>
        <br />
      </mat-card-content>
    </mat-card>
  `,
})
export class StatusSummaryComponent {
  @Input() runningPublishersCount: number | null = 0;
  @Input() pausedPublishersCount: number | null = 0;
  @Input() stoppedPublishersCount: number | null = 0;

  @Input() runningSubscribersCount: number | null = 0;
  @Input() pausedSubscribersCount: number | null = 0;
  @Input() stoppedSubscribersCount: number | null = 0;

  @Output() filterPublishersByStatusRequested = new EventEmitter<TraderStatus | null>();
  @Output() filterSubscribersByStatusRequested = new EventEmitter<TraderStatus | null>();

  get TraderStatus(): typeof TraderStatus {
    return TraderStatus;
  }
}
