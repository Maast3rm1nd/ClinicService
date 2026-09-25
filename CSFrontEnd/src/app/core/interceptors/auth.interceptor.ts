import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { TokenStorageService } from '../services/token-storage.service';

/** Attaches the bearer token to outgoing requests and refreshes it once on a 401 response. */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const tokenStorage = inject(TokenStorageService);

  const isAuthEndpoint = req.url.includes('/auth/login') || req.url.includes('/auth/refresh');
  const token = tokenStorage.accessToken;
  const authorizedRequest = token && !isAuthEndpoint
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authorizedRequest).pipe(
    catchError((error: unknown) => {
      if (
        error instanceof HttpErrorResponse &&
        error.status === 401 &&
        !isAuthEndpoint &&
        tokenStorage.refreshToken
      ) {
        return authService.refresh().pipe(
          switchMap((tokens) =>
            next(req.clone({ setHeaders: { Authorization: `Bearer ${tokens.accessToken}` } })),
          ),
          catchError((refreshError) => {
            authService.logout();
            return throwError(() => refreshError);
          }),
        );
      }

      return throwError(() => error);
    }),
  );
};
