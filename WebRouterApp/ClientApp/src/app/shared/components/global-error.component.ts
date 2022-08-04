import { Component } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { AppState } from '../../core/state/app/app.state';
import { AppActions } from '../../core/state/app/app.actions';

@Component({
  selector: 'app-global-error',
  template: `
    <app-error
      [message]="lastError$ | async"
      [showDismiss]="true"
      (dismissRequested)="dismiss()"
    ></app-error>
  `,
})
export class GlobalErrorComponent {
  constructor(private store: Store) {}

  @Select(AppState.lastError)
  readonly lastError$!: Observable<string | null>;

  dismiss(): void {
    this.store.dispatch(new AppActions.DismissError());
  }
}
