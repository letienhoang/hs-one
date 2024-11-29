import { Routes } from '@angular/router';
import { AuthGuard } from 'src/app/shared/services/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'posts',
    pathMatch: 'full',
  },
  {
    path: 'posts',
    loadComponent: () =>
      import('./posts/post.component').then((m) => m.PostComponent),
    data: {
      title: $localize`Posts`,
      requiredPolicy: 'Permissions.Posts.View',
    },
    canActivate: [AuthGuard],
  },
];
