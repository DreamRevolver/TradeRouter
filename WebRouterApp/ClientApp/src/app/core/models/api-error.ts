export interface IApiError {
  tag: string;
}

export function isApiError(error: unknown): error is IApiError {
  return (error as IApiError).tag?.constructor === String;
}

export namespace ErrorTags {
  export const validationError = 'ValidationError';
  export const internalServerError = 'InternalServerError';
  export const invalidCredentialsError = 'InvalidCredentialsError';
  export const invalidUserIdError = 'InvalidUserIdError';
  export const invalidTokenError = 'InvalidTokenError';
  export const subscriberRunningError = 'SubscriberRunningError';
}
