import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { LoginComponent } from './components/login.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AuthLayoutComponent } from './layout/auth-layout.component';
import { NgxsModule } from '@ngxs/store';
import { AuthState } from './state/auth.state';
import { SharedModule } from '../shared/shared.module';
import { RouterModule, Routes } from '@angular/router';
import { RoutePaths } from '../core/constants/routes-paths';
import { AuthHttpInterceptorModule } from './services/auth.http-interceptor';
import { LoggedOutOnlyGuard } from './guards/logged-out-only.guard';

const routes: Routes = [
  {
    path: RoutePaths.login,
    component: LoginComponent,
    canActivate: [LoggedOutOnlyGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [],
})
class AuthRoutingModule {}

@NgModule({
  imports: [
    HttpClientModule, //
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    AuthRoutingModule,
    AuthHttpInterceptorModule.forRoot(),
    NgxsModule.forFeature([AuthState]),
    SharedModule,
  ],
  declarations: [AuthLayoutComponent, LoginComponent],
})
export class AuthModule {}
