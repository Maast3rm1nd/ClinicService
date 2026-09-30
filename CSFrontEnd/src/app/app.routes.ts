import { Routes } from '@angular/router';
import { adminGuard, authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then((m) => m.LoginComponent),
    title: 'Login — Clinic',
  },
  {
    path: 'set-password',
    loadComponent: () =>
      import('./features/auth/set-password/set-password.component').then(
        (m) => m.SetPasswordComponent,
      ),
    title: 'Set password — Clinic',
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
        path: 'security',
        loadComponent: () =>
          import('./features/auth/security/security.component').then((m) => m.SecurityComponent),
        title: 'Account security — Clinic',
      },
      {
        path: 'doctors',
        loadComponent: () => import('./features/doctors/doctors.component').then((m) => m.DoctorsComponent),
        canActivate: [adminGuard],
        title: 'Doctors — Clinic',
      },
      {
        path: 'policies',
        loadComponent: () =>
          import('./features/policies/policies.component').then((m) => m.PoliciesComponent),
        canActivate: [adminGuard],
        title: 'Policies — Clinic',
      },
      {
        path: 'insurance-providers',
        loadComponent: () =>
          import('./features/insurance-providers/insurance-providers.component').then(
            (m) => m.InsuranceProvidersComponent,
          ),
        canActivate: [adminGuard],
        title: 'Insurance providers — Clinic',
      },
      {
        path: 'specialisations',
        loadComponent: () =>
          import('./features/specialisations/specialisations.component').then(
            (m) => m.SpecialisationsComponent,
          ),
        canActivate: [adminGuard],
        title: 'Specialisations — Clinic',
      },
      {
        path: 'medical-cards',
        loadComponent: () =>
          import('./features/medical-cards/medical-cards.component').then((m) => m.MedicalCardsComponent),
        title: 'Medical cards — Clinic',
      },
      {
        path: 'patients',
        loadComponent: () =>
          import('./features/patients/patients.component').then((m) => m.PatientsComponent),
        title: 'Patients — Clinic',
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
