import { Routes } from '@angular/router';

export const AUTHENTICATION_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('../pages/login/login').then((component) => component.Login),
    title: 'Sign in | Task Manager',
  },
];
