import { ErrorHandler, NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FooterComponent } from './layouts/footer.component';
import { MainLayoutComponent } from './layouts/main-layout.component';
import { ToolbarComponent } from './layouts/toolbar.component';
import { PageNotFoundComponent } from './components/page-not-found.component';
import { AppErrorHandler } from './services/app-error-handler';
import { SharedModule } from '../shared/shared.module';
import { AppState } from './state/app/app.state';
import { NgxsModule } from '@ngxs/store';
import { TradingState } from '../trading/state/trading.state';
import { DashboardPublishersState } from '../dashboard/state/dashboard-publishers.state';
import { DashboardSubscribersState } from '../dashboard/state/dashboard-subscribers.state';
import { SideNavComponent } from './layouts/sidenav.component';

@NgModule({
  imports: [
    NgxsModule.forFeature([
      AppState,
      DashboardPublishersState,
      DashboardSubscribersState,
      TradingState,
    ]),
    CommonModule,
    RouterModule,
    SharedModule,
  ],
  declarations: [
    MainLayoutComponent, //
    SideNavComponent,
    ToolbarComponent,
    FooterComponent,
    PageNotFoundComponent,
  ],
  providers: [
    { provide: ErrorHandler, useClass: AppErrorHandler }, //
  ],
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    // `@SkipSelf` makes sure we skip current injector (as it provides ourselves as CoreModule),
    // and work up from the parent injector looking for one providing `CoreModule`.
    // If we do find it, that means `CoreModule` has already been imported at least once
    // (probably in `AppModule`).
    // As `CoreModule` must only be imported once in `AppModule`, we throw an error
    // to prevent importing it again.
    // If we don't find any previous imports, that means everything is OK,
    // and `@Optional` makes sure DI doesn't throw any errors in such a case.
    if (!parentModule) {
      return;
    }

    throw new Error(
      `CoreModule has already been loaded. Import CoreModule module in the AppModule only.`
    );
  }
}
