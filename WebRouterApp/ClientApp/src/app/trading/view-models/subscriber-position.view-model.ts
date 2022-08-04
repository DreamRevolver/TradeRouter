import { IPublisherPositionViewModel } from './publisher-position.view-model';

export interface ISubscriberPositionViewModel extends IPublisherPositionViewModel {
  points: number;
  mid: number;
  diff: number;
  follows: string;
  entryPriceDelta: number;
}
