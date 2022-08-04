import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ErrorSummaryComponent } from './components/error-summary.component';
import { StatusSummaryComponent } from './components/status-summary.component';
import { PublishersComponent } from './components/publishers.component';
import { SubscribersComponent } from './components/subscribers.component';
import { SharedModule } from '../shared/shared.module';
import { TradersComponent } from './components/traders.component';
import { RouterModule, Routes } from '@angular/router';
import { DashboardPageComponent } from './components/dashboard-page.component';

const routes: Routes = [
  {
    path: '',
    component: DashboardPageComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [],
})
class DashboardRoutingModule {}

@NgModule({
  imports: [
    CommonModule, //
    DashboardRoutingModule,
    SharedModule,
  ],

  declarations: [
    DashboardPageComponent, //
    ErrorSummaryComponent,
    StatusSummaryComponent,
    TradersComponent,
    PublishersComponent,
    SubscribersComponent,
  ],
})
export class DashboardModule {}
