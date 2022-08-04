import { IPublisherOrderViewModel } from './publisher-order.view-model';

export interface ISubscriberOrderViewModel extends IPublisherOrderViewModel {
  points: number;
  mid: number;
  diff: number;
  follows: string;
  type: string;
}
