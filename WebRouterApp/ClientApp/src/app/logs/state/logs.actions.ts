import { ActionWith } from '../../shared/state/action-with-payload';

export namespace LogsApiActions {
  const group = '[Logs API]';
  export class GetAll extends ActionWith<void> {
    static type = `${group} Get All`;
  }
}

export namespace LogsActions {
  const group = '[Logs]';

  export class StartPolling extends ActionWith<void> {
    static type = `${group} Start Polling`;
  }
  export class StopPolling extends ActionWith<void> {
    static type = `${group} Stop Polling`;
  }
}
