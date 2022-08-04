import { MatTable, MatTableDataSource } from '@angular/material/table';
import { Observable, Subscription } from 'rxjs';

export class RxMatTableDataSource<TRow> extends MatTableDataSource<TRow> {
  constructor(rows$?: Observable<TRow[]>) {
    super();
    this.rows$ = rows$;
  }

  public table: MatTable<TRow> | null = null;

  set rows$(rows$: Observable<TRow[]> | null | undefined) {
    this.subscription?.unsubscribe();
    this.data = [];

    if (!rows$) {
      this.table?.renderRows();
      return;
    }

    this.subscription = rows$.subscribe((rows) => {
      this.data = rows;
      this.table?.renderRows();
    });
  }

  private subscription: Subscription | null = null;
  disconnect(): void {
    this.subscription?.unsubscribe();
    super.disconnect();
  }
}
