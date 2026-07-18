import { Routes } from '@angular/router';

export const USER_TASK_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('../pages/task-list/task-list').then((component) => component.TaskList),
    title: 'Tasks | Task Manager',
  },
];
