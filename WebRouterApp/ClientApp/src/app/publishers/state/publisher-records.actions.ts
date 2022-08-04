import { ActionWith } from '../../shared/state/action-with-payload';
import { IPublisherRecord } from '../models/publisher-record';

export namespace PublisherRecordsApiActions {
  const group = '[Publisher Records API]';

  export class GetRecords extends ActionWith<void> {
    static type = `${group} Get Records`;
  }

  export class Update extends ActionWith<{ record: Partial<IPublisherRecord> }> {
    static type = `${group} Update`;
  }
}
