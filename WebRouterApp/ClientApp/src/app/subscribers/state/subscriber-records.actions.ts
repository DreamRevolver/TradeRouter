import { ActionWith } from '../../shared/state/action-with-payload';
import { Guid } from '../../shared/types/guid';
import { ISubscriberRecord } from '../models/subscriber-record';

export namespace SubscriberRecordsActions {
  const group = '[Subscriber Records]';

  export class Add extends ActionWith<void> {
    static type = `${group} Add`;
  }

  export class Edit extends ActionWith<{ record: Partial<ISubscriberRecord> | null }> {
    static type = `${group} Edit`;
  }

  export class Delete extends ActionWith<{ id: Guid | null }> {
    static type = `${group} Delete`;
  }

  export class DismissError extends ActionWith<void> {
    static type = `${group} Dismiss Error`;
  }
}

export namespace SubscriberRecordsApiActions {
  const group = '[Subscriber Records API]';

  export class GetRecords extends ActionWith<void> {
    static type = `${group} Get Records`;
  }
}
