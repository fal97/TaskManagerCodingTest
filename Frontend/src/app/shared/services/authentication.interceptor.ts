import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { AuthenticationService } from '../../features/authentication/services';

export const authenticationInterceptor: HttpInterceptorFn = (request, next) => {
  const router = inject(Router);
  const authenticationService = inject(AuthenticationService);
  const accessToken = authenticationService.getAccessToken();
  const isAnonymousRequest =
    request.url.endsWith('/api/auth/login') || request.url.endsWith('/api/auth/register');
  const authenticatedRequest = accessToken && !isAnonymousRequest
    ? request.clone({
        setHeaders: { Authorization: `Bearer ${accessToken}` },
      })
    : request;

  return next(authenticatedRequest).pipe(
    catchError((error) => {
      if (error.status === 401 && !isAnonymousRequest) {
        authenticationService.clearSession();
        void router.navigate(['/login'], {
          queryParams: { returnUrl: router.url === '/login' ? '/tasks' : router.url },
        });
      }

      return throwError(() => error);
    }),
  );
};
