import { ActionWith } from '../../shared/state/action-with-payload';
import { ILoginApiCommand } from '../models/login';
import { IRefreshTokenApiResponse } from '../models/refresh';

export namespace AuthActions {
  const group = '[Auth]';

  export class RedirectToLogin extends ActionWith<{ originalUrl: string }> {
    static type = `${group} Redirect to Login`;
  }
  export class RedirectToDefault extends ActionWith<void> {
    static type = `${group} Redirect to Default`;
  }
  export class LogIn extends ActionWith<ILoginApiCommand> {
    static type = `${group} Log In`;
  }
  export class RefreshToken extends ActionWith<IRefreshTokenApiResponse> {
    static type = `${group} Refresh Token`;
  }
  export class LogOut extends ActionWith<void> {
    static type = `${group} Log Out`;
  }
}
