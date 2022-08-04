import { IUserModel } from '../../users/models/user.model';
import { ErrorTags, IApiError, isApiError } from '../../core/models/api-error';

export interface ILoginApiCommand {
  username: string;
  password: string;
}

export interface ILoginApiResponse {
  user: IUserModel;
  accessToken: string;
}

export interface IInvalidCredentialsError extends IApiError {
  tag: typeof ErrorTags.invalidCredentialsError;
  message: string;
}

export function isInvalidCredentialsError(error: unknown): error is IInvalidCredentialsError {
  return isApiError(error) && error.tag === ErrorTags.invalidCredentialsError;
}
