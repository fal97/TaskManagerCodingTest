import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideNativeDateAdapter } from '@angular/material/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import { routes } from './app.routes';
import { authenticationInterceptor } from './shared/services';
import { AuthenticationService } from './features/authentication/services';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideAppInitializer(() => inject(AuthenticationService).initialize()),
    provideHttpClient(withInterceptors([authenticationInterceptor])),
    provideNativeDateAdapter(),
    provideAnimationsAsync(),
    provideRouter(routes)
  ]
};
