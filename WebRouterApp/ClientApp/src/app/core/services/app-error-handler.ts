import { ErrorHandler, Injectable } from '@angular/core';
import { Store } from '@ngxs/store';
import { AppActions } from '../state/app/app.actions';

@Injectable()
export class AppErrorHandler extends ErrorHandler {
  constructor(private store: Store) {
    super();
  }

  handleError(error: unknown): void {
    // super.handleError(error);
    this.store.dispatch(new AppActions.HandleError({ error }));
  }
}
