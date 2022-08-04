import {
  Component,
  ContentChild,
  HostBinding,
  Input,
  OnDestroy,
  OnInit,
  Optional,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import {
  MatCellDef,
  MatColumnDef,
  MatFooterCellDef,
  MatHeaderCellDef,
  MatTable,
} from '@angular/material/table';

export type CellValueNeededFn = (data: Record<string, unknown>, name: string) => string;

@Component({
  selector: 'app-column',
  template: `
    <ng-container matColumnDef>
      <th mat-header-cell *matHeaderCellDef>{{ label || capitalize(name) }}</th>
      <td mat-cell *matCellDef="let row">{{ getCellValue(row) }}</td>
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
export class ColumnComponent implements OnDestroy, OnInit {
  @Input() name = '';
  @Input() label: string | null = null;
  @Input() align: 'before' | 'after' = 'before';
  @Input() sortable = false;
  @Input() cellValueNeeded: CellValueNeededFn | null = null;

  constructor(@Optional() public table: MatTable<unknown>) {}

  @ViewChild(MatColumnDef, { static: true }) columnDef!: MatColumnDef;
  @ViewChild(MatCellDef, { static: true }) cellDef!: MatCellDef;
  @ViewChild(MatHeaderCellDef, { static: true }) headerCellDef!: MatHeaderCellDef;
  @ViewChild(MatFooterCellDef, { static: true }) footerCellDef!: MatFooterCellDef;

  @ContentChild('cell', { static: false })
  cellTemplate: TemplateRef<unknown> | null = null;

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

  capitalize(value: string): string {
    return value.charAt(0).toUpperCase() + value.slice(1);
  }

  getCellValue(row: Record<string, unknown>): unknown {
    return this.cellValueNeeded ? this.cellValueNeeded(row, this.name) : row[this.name];
  }
}

// This attempt to build `ColumnComponent` based on `ColumnTemplateComponent` ends up with:
// ```
// Error: Could not find column with id "...".
//     at getTableUnknownColumnError (table.js:1078) [angular]
//     blah-blah-blah...
// ```
//
// @Component({
//   selector: 'app-column',
//   template: `
//     <app-column-template [name]="name" [label]="label" [sortable]="sortable" [align]="align">
//       <ng-template #cell let-row> {{ getCellValue(row) }} </ng-template>
//     </app-column-template>
//   `,
// })
// export class ColumnComponent {
//   @Input() name = '';
//   @Input() label: string | null = null;
//   @Input() align: 'before' | 'after' = 'before';
//   @Input() sortable = false;
//
//   @Input() cellValueNeeded: CellValueNeededFn | null = null;
//
//   getCellValue(row: Record<string, unknown>): unknown {
//     return this.cellValueNeeded ? this.cellValueNeeded(row, this.name) : row[this.name];
//   }
// }
