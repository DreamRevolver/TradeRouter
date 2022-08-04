import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Select, Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { AuthState } from '../state/auth.state';
import { AuthActions } from '../state/auth.actions';

@Component({
  selector: 'app-login-page',
  template: `
    <div class="container column">
      <app-global-error></app-global-error>
      <form class="content">
        <mat-card>
          <app-spinner
            [name]="'app-login-page-spinner'"
            [showSpinner]="(isBusy$ | async) || false"
          ></app-spinner>
          <mat-card-title>Welcome</mat-card-title>
          <mat-card-content [formGroup]="loginForm">
            <div>
              <p>
                <mat-form-field>
                  <input
                    appAutofocus
                    type="text"
                    matInput
                    placeholder="Username"
                    formControlName="username"
                    required
                  />
                  <mat-error>Please provide a username</mat-error>
                </mat-form-field>
              </p>

              <p>
                <mat-form-field>
                  <input
                    type="password"
                    matInput
                    placeholder="Password"
                    formControlName="password"
                    required
                  />
                  <mat-error> Please provide a password</mat-error>
                </mat-form-field>
              </p>

              <p *ngIf="lastError$ | async as lastError">
                <app-error [message]="lastError"></app-error>
              </p>

              <div class="button">
                <button type="submit" mat-button (click)="login()">Login</button>
              </div>
            </div>
          </mat-card-content>
        </mat-card>
      </form>
    </div>
  `,
  styles: [
    `
      .container {
        position: absolute;
        top: 0;
        bottom: 0;
        left: 0;
        right: 0;
      }

      .content {
        display: flex;
        justify-content: center;
        margin: 100px 0;
      }

      .mat-form-field {
        width: 100%;
        min-width: 300px;
      }

      mat-card-title,
      mat-card-content {
        display: flex;
        justify-content: center;
      }

      .error {
        padding: 16px;
        width: 300px;
        color: white;
        background-color: darkred;
      }

      .button {
        display: flex;
        justify-content: flex-end;
      }
    `,
  ],
})
export class LoginComponent {
  constructor(private store: Store) {}

  @Select(AuthState.lastError)
  lastError$!: Observable<string | null>;

  @Select(AuthState.isBusy)
  isBusy$!: Observable<boolean>;

  loginForm = new FormGroup({
    username: new FormControl(''),
    password: new FormControl(''),
  });

  login(): void {
    if (!this.loginForm.valid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.store.dispatch(new AuthActions.LogIn(this.loginForm.value));
  }
}
