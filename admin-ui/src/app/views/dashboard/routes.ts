import { Routes } from '@angular/router';
import { AuthGuard } from 'src/app/shared/services/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./dashboard.component').then((m) => m.DashboardComponent),
    data: {
      title: $localize`Dashboard`,
      requiredPolicy: 'Permissions.Dashboard.View',
    },
    canActivate: [AuthGuard],
  },
];
