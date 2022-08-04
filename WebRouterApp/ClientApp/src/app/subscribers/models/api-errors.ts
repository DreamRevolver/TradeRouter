import { ErrorTags, IApiError, isApiError } from '../../core/models/api-error';
import { Guid } from '../../shared/types/guid';

export interface ISubscriberRunningError extends IApiError {
  tag: typeof ErrorTags.subscriberRunningError;
  message: string;
  subscriberId: Guid;
}

export function isSubscriberRunningError(error: unknown): error is ISubscriberRunningError {
  return isApiError(error) && error.tag === ErrorTags.subscriberRunningError;
}
