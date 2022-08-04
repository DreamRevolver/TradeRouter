import { ErrorTags, IApiError, isApiError } from '../../core/models/api-error';
import { IUserModel } from '../../users/models/user.model';

// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface IRefreshTokenApiCommand {}

export interface IRefreshTokenApiResponse {
  user: IUserModel;
  accessToken: string;
}

export interface IInvalidUserIdError extends IApiError {
  tag: typeof ErrorTags.invalidUserIdError;
  message: string;
}

export function isInvalidUserIdError(error: unknown): error is IInvalidUserIdError {
  return isApiError(error) && error.tag === ErrorTags.invalidUserIdError;
}

export interface IInvalidTokenError extends IApiError {
  tag: typeof ErrorTags.invalidTokenError;
  message: string;
}

export function isInvalidTokenError(error: unknown): error is IInvalidTokenError {
  return isApiError(error) && error.tag === ErrorTags.invalidTokenError;
}
