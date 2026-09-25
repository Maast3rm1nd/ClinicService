import { Routes } from '@angular/router';
import { adminGuard, authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then((m) => m.LoginComponent),
    title: 'Login — Clinic',
  },
  {
    path: '',
    loadComponent: () => import('./layout/shell/shell.component').then((m) => m.ShellComponent),
    canActivate: [authGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then((m) => m.DashboardComponent),
        title: 'Schedule — Clinic',
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./features/auth/register/register.component').then((m) => m.RegisterComponent),
        canActivate: [adminGuard],
        title: 'New user — Clinic',
      },
      {
        path: 'doctors',
        loadComponent: () => import('./features/doctors/doctors.component').then((m) => m.DoctorsComponent),
        title: 'Doctors — Clinic',
      },
      {
        path: 'policies',
        loadComponent: () =>
          import('./features/policies/policies.component').then((m) => m.PoliciesComponent),
        title: 'Policies — Clinic',
      },
      {
        path: 'insurance-providers',
        loadComponent: () =>
          import('./features/insurance-providers/insurance-providers.component').then(
            (m) => m.InsuranceProvidersComponent,
          ),
        title: 'Insurance providers — Clinic',
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
