import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';

import { AuthenticationService } from '../services';

export const authenticationGuard: CanActivateFn = async (_route, state) => {
  const authenticationService = inject(AuthenticationService);

  if (authenticationService.isAuthenticated()) {
    return true;
  }

  await authenticationService.login({
    redirectUri: `${window.location.origin}${state.url}`,
  });
  return false;
};
