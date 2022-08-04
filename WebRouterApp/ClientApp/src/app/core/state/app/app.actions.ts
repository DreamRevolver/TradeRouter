import { ActionWith } from '../../../shared/state/action-with-payload';

export namespace AppActions {
  const group = '[App]';

  export class HandleError extends ActionWith<{ error: unknown }> {
    static type = `${group} Handle Error`;
  }

  export class DismissError extends ActionWith<void> {
    static type = `${group} Dismiss Error`;
  }
}
