import { Component, EventEmitter, Input, Output } from '@angular/core';
import { animate, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'app-error',
  animations: [
    trigger('showHide', [
      transition(':enter', [
        style({ height: 0, padding: '0 14px' }), //
        animate('0.2s ease-in-out'),
      ]),
      transition(':leave', [
        //
        animate('0.2s ease-in-out', style({ height: 0, padding: '0 14px' })),
      ]),
    ]),
  ],
  template: `
    <div @showHide *ngIf="isVisible" class="row error">
      <i class="fa fa-exclamation-circle"></i>
      <span class="message">{{ message }}</span>
      <span class="flex-spacer"></span>
      <button *ngIf="showDismiss" mat-button (click)="dismissRequested.emit()">DISMISS</button>
    </div>
  `,
  styles: [
    `
      .message {
        margin-left: 10px;
      }

      .error {
        overflow: hidden;
        padding: 14px;
        color: rgb(95, 33, 32);
        background-color: rgb(253, 237, 237);
      }

      i {
        margin-top: 2px;
        color: rgb(239, 83, 80);
      }

      .mat-button-base {
        margin: -8px -8px -8px 8px;
        color: rgb(239, 83, 80);
      }
    `,
  ],
})
export class ErrorComponent {
  @Input() message: string | null = null;
  @Input() showDismiss = false;
  @Output() dismissRequested = new EventEmitter<void>();

  get isVisible(): boolean {
    return this.message !== null;
  }
}
