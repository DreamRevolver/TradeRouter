import {
  Component,
  ContentChild,
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

// https://github.com/angular/components/issues/5889#issuecomment-713618939
@Component({
  selector: 'app-column-template',
  template: `
    <ng-container matColumnDef>
      <th mat-header-cell *matHeaderCellDef>{{ label || capitalize(name) }}</th>
      <td mat-cell *matCellDef="let row">
        <ng-container *ngTemplateOutlet="cellTemplate; context: { $implicit: row }"></ng-container>
      </td>
    </ng-container>
  `,
  // ESLint isn't smart enough to understand,
  // the reasoning behind not using the `host` metadata property on directives
  // doesn't apply to this component.
  // eslint-disable-next-line @angular-eslint/no-host-metadata-property
  host: {
    class: 'column-template cdk-visually-hidden',
    '[attr.ariaHidden]': 'true',
  },
})
export class ColumnTemplateComponent implements OnDestroy, OnInit {
  @Input() name = '';
  @Input() label: string | null = null;
  @Input() align: 'before' | 'after' = 'before';
  @Input() sortable = false;

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
}
