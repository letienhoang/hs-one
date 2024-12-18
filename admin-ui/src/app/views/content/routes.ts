import { Routes } from '@angular/router';
import { AuthGuard } from '../../shared/services/auth.guard';
import '@angular/localize/init';

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
  {
    path: 'post-categories',
    loadComponent: () =>
      import('./post-categories/post-category.component').then(
        (m) => m.PostCategoryComponent
      ),
    data: {
      title: $localize`Post Categories`,
      requiredPolicy: 'Permissions.PostCategories.View',
    },
    canActivate: [AuthGuard],      
  }
];
