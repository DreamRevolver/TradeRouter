import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Store } from '@ngxs/store';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

export class ApiClient {
  constructor(
    private http: HttpClient,
    private store: Store,
    featureUrl: string,
    httpOptions: Record<string, unknown> | null
  ) {
    this.featureApiUrl = `${environment.apiUrl}/api/${featureUrl}`;
    this.httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
      ...httpOptions,
    };
  }

  private readonly featureApiUrl: string;
  private readonly httpOptions: {
    headers: HttpHeaders;
    [key: string]: unknown;
  };

  post$<TResult>(endpoint: string, body: unknown): Observable<TResult> {
    return this.http
      .post(`${this.featureApiUrl}/${endpoint}`, body, {
        ...this.httpOptions,
        responseType: 'text',
      })
      .pipe(
        map((response) => <TResult>JSON.parse(response, ApiClient.reviveDateTime)),
        catchError((error) => {
          if (error instanceof HttpErrorResponse) {
            error = new HttpErrorResponse({
              error: typeof error.error === 'string' ? { ...JSON.parse(error.error) } : error.error,
              headers: error.headers,
              status: error.status,
              statusText: error.statusText,
              url: error.url || undefined,
            });
          }

          return throwError(error);
        })
      );
  }

  post<TResult>(endpoint: string, body: unknown): Promise<TResult> {
    return this.post$<TResult>(endpoint, body).toPromise();
  }

  private static readonly reIsoDateTime =
    /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*))(?:Z|(\+|-)([\d|:]*))?$/;

  private static reviveDateTime(key: unknown, value: unknown): unknown {
    if (typeof value !== 'string' || !ApiClient.reIsoDateTime.exec(value)) {
      return value;
    }

    return new Date(value);
  }
}

@Injectable({ providedIn: 'root' })
export class ApiClientFactory {
  constructor(private http: HttpClient, private store: Store) {}

  forFeature(featureUrl: string, httpOptions: Record<string, unknown> | null = null): ApiClient {
    return new ApiClient(this.http, this.store, featureUrl, httpOptions);
  }
}

export function isHttpErrorResponse(error: unknown): error is HttpErrorResponse {
  return error instanceof HttpErrorResponse;
}

export function is401Unauthorized(error: unknown): boolean {
  return isHttpErrorResponse(error) && error.status === 401;
}
