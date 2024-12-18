import { Routes } from '@angular/router';
import { AuthGuard } from '../../shared/services/auth.guard';
import '@angular/localize/init';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'users',
    pathMatch: 'full',
  },
  {
    path: 'users',
    loadComponent: () =>
      import('./users/user.component').then((m) => m.UserComponent),
    data: {
      title: $localize`Users`,
      requiredPolicy: 'Permissions.Users.View',
    },
    canActivate: [AuthGuard],
  },
  { 
    path: 'roles',
    loadComponent: () =>
      import('./roles/role.component').then((m) => m.RoleComponent),
    data: {
      title: $localize`Roles`,
      requiredPolicy: 'Permissions.Roles.View',
    },
    canActivate: [AuthGuard],
  } 
];
