import { Routes } from '@angular/router';
import { AuthGuard } from '../../shared/services/auth.guard';
import '@angular/localize/init';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'tramsactions',
    pathMatch: 'full',
  },
  {
    path: 'transactions',
    loadComponent: () =>
      import('./transactions/transactions.component').then((m) => m.TransactionComponent),
    data: {
      title: $localize`Transactions`,
      requiredPolicy: 'Permissions.Royalty.View',
    },
    canActivate: [AuthGuard],
  },
  {
    path: 'royalty-month',
    loadComponent: () =>
      import('./royalty-month/royalty-month.component').then((m) => m.RoyaltyReportMonthComponent),
    data: {
      title: $localize`Royalty Month`,
      requiredPolicy: 'Permissions.Royalty.View',
    },
    canActivate: [AuthGuard],
  },
  {
    path: 'royalty-user',
    loadComponent: () =>
      import('./royalty-user/royalty-user.component').then((m) => m.RoyaltyReportUserComponent),
    data: {
      title: $localize`Royalty User`,
      requiredPolicy: 'Permissions.Royalty.View',
    },
    canActivate: [AuthGuard],
  },
];
