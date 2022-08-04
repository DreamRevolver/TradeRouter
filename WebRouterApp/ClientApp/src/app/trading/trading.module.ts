import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersPanelComponent } from './components/orders-panel.component';
import { PositionsPanelComponent } from './components/positions-panel.components';
import { SharedModule } from '../shared/shared.module';
import { RouterModule, Routes } from '@angular/router';
import { TradingPageComponent } from './components/trading-page.component';

const routes: Routes = [
  {
    path: '',
    component: TradingPageComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [],
})
class TradingRoutingModule {}

@NgModule({
  imports: [
    CommonModule, //
    TradingRoutingModule,
    SharedModule,
  ],

  declarations: [
    TradingPageComponent, //
    OrdersPanelComponent,
    PositionsPanelComponent,
  ],
})
export class TradingModule {}
