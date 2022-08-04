import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CoreModule } from './core/core.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgxsModule, NoopNgxsExecutionStrategy } from '@ngxs/store';
import { environment } from '../environments/environment';
import { AuthModule } from './auth/auth.module';
import { NgxsStoragePluginModule, StorageOption } from '@ngxs/storage-plugin';
import { AuthState } from './auth/state/auth.state';
import { ScopedSubscriptionModule } from './shared/scoped-subscription/scoped-subscription.module';
import { NgxsLogOutPluginModule } from './core/state/logout.plugin';
import { RouterModule, Routes } from '@angular/router';
import { RoutePaths } from './core/constants/routes-paths';
import { MainLayoutComponent } from './core/layouts/main-layout.component';
import { PageNotFoundComponent } from './core/components/page-not-found.component';
import { LoggedInOnlyGuard } from './auth/guards/logged-in-only.guard';
import { NgxsActionsExecutingModule } from '@ngxs-labs/actions-executing';

const routes: Routes = [
  {
    path: RoutePaths.empty,
    redirectTo: RoutePaths.dashboard,
    pathMatch: 'full',
  },
  {
    path: RoutePaths.empty,
    component: MainLayoutComponent,
    canActivate: [LoggedInOnlyGuard],
    children: [
      {
        path: RoutePaths.dashboard,
        loadChildren: () => import('./dashboard/dashboard.module').then((m) => m.DashboardModule),
      },
      {
        path: RoutePaths.trading,
        loadChildren: () => import('./trading/trading.module').then((m) => m.TradingModule),
      },
      {
        path: RoutePaths.logs,
        loadChildren: () => import('./logs/logs.module').then((m) => m.LogsModule),
      },
      {
        path: RoutePaths.subscriberRecords,
        loadChildren: () =>
          import('./subscribers/subscriber-records.module').then((m) => m.SubscriberRecordsModule),
      },
      {
        path: RoutePaths.publisherRecords,
        loadChildren: () =>
          import('./publishers/publisher-records.module').then((m) => m.PublisherRecordsModule),
      },
    ],
  },
  // Fallback when no prior route is matched
  { path: RoutePaths.any, component: PageNotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [],
})
class AppRoutingModule {}

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule, //
    BrowserAnimationsModule,
    FontAwesomeModule,
    NgxsModule.forRoot([], {
      developmentMode: !environment.production,
      selectorOptions: {
        injectContainerState: false,
        suppressErrors: false,
      },
      executionStrategy: NoopNgxsExecutionStrategy,
    }),
    NgxsLogOutPluginModule.forRoot(),
    NgxsStoragePluginModule.forRoot({
      key: AuthState.storageKeys,
      storage: StorageOption.SessionStorage,
    }),
    NgxsActionsExecutingModule.forRoot(),
    ...environment.imports,
    CoreModule,
    AuthModule,
    AppRoutingModule,
    ScopedSubscriptionModule.forRoot(),
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
