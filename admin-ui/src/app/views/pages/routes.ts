import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '500',
    pathMatch: 'full',
  },
  {
    path: '403',
    loadComponent: () => import('./page403/page403.component').then(m => m.Page403Component),
    data: {
      title: 'Page 403'
    }
  },
  {
    path: '404',
    loadComponent: () => import('./page404/page404.component').then(m => m.Page404Component),
    data: {
      title: 'Page 404'
    }
  },
  {
    path: '500',
    loadComponent: () => import('./page500/page500.component').then(m => m.Page500Component),
    data: {
      title: 'Page 500'
    }
  },
];
