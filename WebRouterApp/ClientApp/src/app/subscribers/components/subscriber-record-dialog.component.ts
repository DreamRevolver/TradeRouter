import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ISubscriberRecord } from '../models/subscriber-record';
import { CoeffKindsRenderer } from '../services/coeff-kinds.renderer';

@Component({
  selector: 'app-subscriber-record-dialog',
  template: ` <form (ngSubmit)="save()" ngNativeValidate>
    <h1 mat-dialog-title>"{{ record.name }}" Settings</h1>
    <div mat-dialog-content>
      <mat-form-field appearance="fill">
        <mat-label>Name</mat-label>
        <input matInput [(ngModel)]="record.name" name="name" required />
      </mat-form-field>
      <mat-form-field appearance="fill">
        <mat-label>Description</mat-label>
        <textarea matInput [(ngModel)]="record.description" name="description"></textarea>
      </mat-form-field>
      <mat-form-field appearance="fill">
        <mat-label>Api Key</mat-label>
        <input matInput [(ngModel)]="record.apiKey" name="apiKey" required />
      </mat-form-field>
      <mat-form-field appearance="fill">
        <mat-label>Api Secret</mat-label>
        <input matInput [(ngModel)]="record.apiSecret" name="apiSecret" required />
      </mat-form-field>
      <mat-form-field appearance="fill">
        <mat-select [value]="record.coeffKind">
          <ng-container *ngFor="let coeffKind of coeffKindRenderer.coeffKinds()">
            <mat-option [value]="coeffKind">
              {{ coeffKindRenderer.stringify(coeffKind) }}
            </mat-option>
          </ng-container>
        </mat-select>
      </mat-form-field>
      <mat-form-field appearance="fill">
        <mat-label>Coefficient</mat-label>
        <input
          type="number"
          min="0.0"
          step="0.01"
          matInput
          [(ngModel)]="record.multiplier"
          name="multiplier"
        />
      </mat-form-field>
    </div>
    <div mat-dialog-actions>
      <button mat-button cdkFocusInitial>Save</button>
      <button mat-button type="button" (click)="cancel()">Cancel</button>
    </div>
  </form>`,
  styles: [
    `
      .mat-form-field {
        width: 100%;
      }
    `,
  ],
})
export class SubscriberRecordDialogComponent {
  isCancel = false;

  constructor(
    public dialogRef: MatDialogRef<ISubscriberRecord>,
    @Inject(MAT_DIALOG_DATA) public record: ISubscriberRecord,
    public coeffKindRenderer: CoeffKindsRenderer
  ) {}

  save(): void {
    this.dialogRef.close(this.record);
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
