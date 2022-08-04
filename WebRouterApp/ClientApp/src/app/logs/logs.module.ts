import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { RouterModule, Routes } from '@angular/router';
import { LogRecordsComponent } from './components/log-records.component';
import { LogPageComponent } from './components/log-page.component';
import { NgxsModule } from '@ngxs/store';
import { LogsState } from './state/logs.state';

const routes: Routes = [
  {
    path: '',
    component: LogPageComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [],
})
class LogsRoutingModule {}

@NgModule({
  imports: [
    CommonModule, //
    SharedModule,
    LogsRoutingModule,
    NgxsModule.forFeature([LogsState]),
  ],
  declarations: [LogPageComponent, LogRecordsComponent],
})
export class LogsModule {}
