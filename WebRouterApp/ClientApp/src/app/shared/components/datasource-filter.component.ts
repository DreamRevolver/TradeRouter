import { AfterViewInit, Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';

export interface FilterSpec {
  attrName: string;
  value: string;
}

@Component({
  selector: 'app-ds-filter',

  template: `
    <div class="row">
      <button class="filter-button" mat-stroked-button (click)="showNewFilter()">
        <mat-icon>filter_alt</mat-icon>
      </button>
      <mat-chip-list class="filter-chip-list">
        <mat-chip *ngFor="let filter of filters" (removed)="removeFilter(filter)">
          {{ filter.attrName + ' : ' + filter.value }}
          <button matChipRemove>
            <mat-icon>cancel</mat-icon>
          </button>
        </mat-chip>
        <div class="mat-chip mat-standard-chip row" *ngIf="showNewFilterChip">
          <form (ngSubmit)="addFilter(filterAttrNameIn.value, filterValueIn.value)"  >
            <select #filterAttrNameIn appAutofocus (keydown.esc)="closeFilter()">
              <option *ngFor="let attrName of filterAttributes" value="{{ attrName }}"  >
                {{ attrName }}
              </option>
            </select>
            <input matInput type="text" #filterValueIn  (keydown.esc)="closeFilter()" />
            <button mat-icon-button class="chip-button">
              <mat-icon matChipTrailingIcon>done</mat-icon>
            </button>
            <button
              mat-icon-button
              class="chip-button"
              type="button"
              (click)="showNewFilterChip = false"
            >
              <mat-icon matChipTrailingIcon>cancel</mat-icon>
            </button>
          </form>
        </div>
      </mat-chip-list>
    </div>
  `,
  styles: [
    `
      .filter-button {
        margin: 8px 8px 8px 0;
      }
      .chip-button {
        max-width: 24px;
        bottom: 4px;
      }
      .filter-chip-list {
        margin: 8px 8px 8px 0;
      }
    `,
  ],
})
export class DataSourceFilterComponent<T> implements OnChanges, AfterViewInit {
  @Input() dataSource: MatTableDataSource<T> = new MatTableDataSource<T>();
  @Input() filters: FilterSpec[] = [];
  filterAttributes: string[] = [];
  filterValue = '';
  filterAttrName = '';
  showNewFilterChip = false;

  ngOnChanges(changes: SimpleChanges): void {
    console.log(`DataSourceFilterComponent change: ${changes}`);

    if (changes['filters']) {
      this.updateFilter();
    }
    if (changes['dataSource']) {
      this.updateFilterAttributes();
    }
  }

  updateFilterAttributes(): void {
    if (this.dataSource.data.length > 0)
      this.filterAttributes = Object.keys(this.dataSource.data[0]);
  }

  ngAfterViewInit(): void {
    this.dataSource.filterPredicate = (it) => {
      const logicalOrResults = new Map<string, boolean>();
      for (const filter of this.filters) {
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        const logicalOrResult = logicalOrResults.get(filter.attrName);
        const value = (<any>it)[filter.attrName];
        // (undefined || true) === true
        // (undefined || false) === false
        logicalOrResults.set(filter.attrName, logicalOrResult || value === filter.value);
      }

      for (const logicalOrResult of logicalOrResults.values()) {
        if (!logicalOrResult) return false;
      }

      return true;
    };

    this.updateFilter();
  }

  updateFilter(): void {
    this.dataSource.filter = ' ';
  }

  showNewFilter(): void {
    this.showNewFilterChip = !this.showNewFilterChip;
    if (this.showNewFilterChip) this.updateFilterAttributes();
  }

  addFilter(attrName: string, value: string): void {
    this.filters.push({ attrName: attrName, value: value });
    this.showNewFilterChip = false;
  }

  removeFilter(filter: FilterSpec): void {
    const index = this.filters.indexOf(filter);
    if (index >= 0) {
      this.filters.splice(index, 1);
      this.updateFilter();
    }
  }

  closeFilter():void{
    this.showNewFilterChip = false;
  }
}
