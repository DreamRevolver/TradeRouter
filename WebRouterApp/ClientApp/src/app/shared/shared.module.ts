import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppMaterialModule } from './material/app-material.module';
import { TimeSpanPipe } from './pipes/time-span.pipe';
import { GlobalErrorComponent } from './components/global-error.component';
import { ErrorComponent } from './components/error.component';
import { ColumnTemplateComponent } from './components/column-template.component';
import { ColumnComponent } from './components/column.component';
import { ColumnCheckboxComponent } from './components/column-checkbox.component';
import { DecimalFractionPipe, PriceDecimalAutoPipe } from './pipes/decimal-fraction.pipe';
import { UnixSecondsToDatePipe } from './pipes/unix-seconds-to-date.pipe';
import { AutoFocusDirective } from './directives/autofocus.directive';
import { DataSourceFilterComponent } from './components/datasource-filter.component';
import { FormsModule } from '@angular/forms';
import { ConfirmDialogComponent } from './components/confirm-dialog.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { DiffStringGeneratorPipe } from './pipes/diff-processor.pipe';
import { SpinnerComponent } from './components/spinner.component';

const declarations = [
  TimeSpanPipe, //
  UnixSecondsToDatePipe,
  DecimalFractionPipe,
  PriceDecimalAutoPipe,
  DiffStringGeneratorPipe,
  AutoFocusDirective,
  ErrorComponent,
  GlobalErrorComponent,
  ColumnTemplateComponent,
  ColumnComponent,
  ColumnCheckboxComponent,
  DataSourceFilterComponent,
  ConfirmDialogComponent,
  SpinnerComponent,
];

@NgModule({
  imports: [
    CommonModule, //
    AppMaterialModule,
    FormsModule,
    NgxSpinnerModule,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  declarations,
  exports: [...declarations, AppMaterialModule, NgxSpinnerModule],
})
export class SharedModule {}
