export interface IRefreshTokenError {
  readonly originalError: unknown;
  readonly forDeferredRequest: boolean;
}

export function isRefreshTokenError(error: unknown): error is IRefreshTokenError {
  const refreshTokenError = error as IRefreshTokenError;
  return (
    refreshTokenError?.originalError !== undefined &&
    refreshTokenError?.forDeferredRequest !== undefined
  );
}
