import { Routes } from '@angular/router';
import { authenticationGuard } from './features/authentication/guards/authentication.guard';

export const routes: Routes = [
  {
    path: 'tasks',
    canActivate: [authenticationGuard],
    loadChildren: () =>
      import('./features/user-tasks/routes/user-task.routes').then(
        (routes) => routes.USER_TASK_ROUTES,
      ),
  },
  {
    path: 'login',
    loadChildren: () =>
      import('./features/authentication/routes/authentication.routes').then(
        (routes) => routes.AUTHENTICATION_ROUTES,
      ),
  },
  { path: '', pathMatch: 'full', redirectTo: 'tasks' },
  { path: '**', redirectTo: 'tasks' },
];
