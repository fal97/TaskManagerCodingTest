import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { from, switchMap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthenticationService } from '../../features/authentication/services';

export const authenticationInterceptor: HttpInterceptorFn = (request, next) => {
  const authenticationService = inject(AuthenticationService);

  if (!request.url.startsWith(environment.apiBaseUrl)) {
    return next(request);
  }

  return from(authenticationService.getValidAccessToken()).pipe(
    switchMap((accessToken) => {
      const authenticatedRequest = accessToken
        ? request.clone({
            setHeaders: { Authorization: `Bearer ${accessToken}` },
          })
        : request;

      return next(authenticatedRequest);
    }),
  );
};
