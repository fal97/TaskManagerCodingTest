import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { AuthenticationService } from '../../features/authentication/services';

export const authenticationInterceptor: HttpInterceptorFn = (request, next) => {
  const router = inject(Router);
  const authenticationService = inject(AuthenticationService);
  const authenticatedRequest = request.clone({ withCredentials: true });

  return next(authenticatedRequest).pipe(
    catchError((error) => {
      const isLoginRequest = request.url.endsWith('/api/auth/login');

      if (error.status === 401 && !isLoginRequest) {
        authenticationService.clearSession();
        void router.navigate(['/login'], {
          queryParams: { returnUrl: router.url === '/login' ? '/tasks' : router.url },
        });
      }

      return throwError(() => error);
    }),
  );
};
