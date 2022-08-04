import {
  Component,
  EventEmitter,
  HostBinding,
  Input,
  OnDestroy,
  OnInit,
  Optional,
  Output,
  ViewChild,
} from '@angular/core';
import {
  MatCellDef,
  MatColumnDef,
  MatFooterCellDef,
  MatHeaderCellDef,
  MatTable,
} from '@angular/material/table';

export type IsSelectedFn<TRow> = (row: TRow) => boolean;

@Component({
  selector: 'app-column-checkbox',
  template: `
    <ng-container matColumnDef>
      <th mat-header-cell *matHeaderCellDef>
        <mat-checkbox
          (change)="$event ? toggleAllRequested.emit() : null"
          [checked]="allSelected"
          [indeterminate]="anySelected && !allSelected"
        >
        </mat-checkbox>
      </th>
      <td mat-cell *matCellDef="let row">
        <mat-checkbox
          (click)="$event.stopPropagation()"
          (change)="toggleRequested.emit(row)"
          [checked]="isSelected(row)"
        >
        </mat-checkbox>
      </td>
    </ng-container>
  `,
  // ESLint isn't smart enough to understand,
  // the reasoning behind not using the `host` metadata property on directives
  // doesn't apply to this component.
  // eslint-disable-next-line @angular-eslint/no-host-metadata-property
  host: {
    class: 'column cdk-visually-hidden',
    '[attr.ariaHidden]': 'true',
  },
})
@HostBinding()
export class ColumnCheckboxComponent<TRow> implements OnDestroy, OnInit {
  @Input() name = '';

  @Input() anySelected = false;
  @Input() allSelected = false;
  @Input() isSelected: IsSelectedFn<TRow> = () => false;

  @Output() toggleRequested = new EventEmitter<TRow>();
  @Output() toggleAllRequested = new EventEmitter<void>();

  constructor(@Optional() public table: MatTable<unknown>) {}

  @ViewChild(MatColumnDef, { static: true }) columnDef!: MatColumnDef;
  @ViewChild(MatCellDef, { static: true }) cellDef!: MatCellDef;
  @ViewChild(MatHeaderCellDef, { static: true }) headerCellDef!: MatHeaderCellDef;
  @ViewChild(MatFooterCellDef, { static: true }) footerCellDef!: MatFooterCellDef;

  ngOnInit(): void {
    if (this.table && this.columnDef) {
      this.columnDef.name = this.name;
      this.columnDef.cell = this.cellDef;
      this.columnDef.headerCell = this.headerCellDef;
      this.columnDef.footerCell = this.footerCellDef;
      this.table.addColumnDef(this.columnDef);
    }
  }

  ngOnDestroy(): void {
    if (this.table) {
      this.table.removeColumnDef(this.columnDef);
    }
  }
}
