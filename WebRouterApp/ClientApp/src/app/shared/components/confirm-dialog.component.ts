import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-confirm-dialog',
  template: `<h1 mat-dialog-title>
      {{ title }}
    </h1>

    <div mat-dialog-content>
      <p>{{ message }}</p>
    </div>

    <div mat-dialog-actions>
      <button mat-button (click)="onConfirm()" cdkFocusInitial>Yes</button>
      <button mat-button (click)="onDismiss()">No</button>
    </div>`,
})
export class ConfirmDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: IConfirmDialogData
  ) {
    this.title = data.title;
    this.message = data.message;
  }

  title: string;
  message: string;

  onConfirm(): void {
    this.dialogRef.close(true);
  }

  onDismiss(): void {
    this.dialogRef.close(false);
  }
}

export interface IConfirmDialogData {
  title: string;
  message: string;
}
