import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-error-summary',
  template: `
    <mat-card class="mat-elevation-z0" >
      <mat-card-subtitle>Errors summary</mat-card-subtitle>
      <mat-card-content>
        <p>
          You have
          <span style="color: red" class="link"
            ><i class="fa fa-bug"></i> {{ errorsOnPublisher || 0 }} errors
          </span>
          on publisher and
          <span class="link" style="color: red" (click)="filterErrorsRequested.emit()">
            <i class="fa fa-bug"></i> {{ errorsOnSubscribers || 0 }} errors
          </span>
          on subscriber nodes.<br />
          <span class="link" style="color: green">0 unsynced positions</span>
          and
          <span class="link" style="color: red">13 unsynced orders</span>
        </p>
      </mat-card-content>
    </mat-card>
  `,
})
export class ErrorSummaryComponent {
  @Input() errorsOnPublisher: number | null = 0;
  @Input() errorsOnSubscribers: number | null = 0;
  @Output() filterErrorsRequested = new EventEmitter<void>();
}
