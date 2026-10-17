import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';

/** Credentials handed over from the login page; kept in navigation state only, never in the URL. */
export interface ChangePasswordNavigationState {
  login: string;
  currentPassword: string;
  twoFactorCode?: string | null;
}

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css',
})
export class ChangePasswordComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  private readonly credentials: ChangePasswordNavigationState | null =
    (this.router.getCurrentNavigation()?.extras.state as ChangePasswordNavigationState | undefined) ??
    null;

  readonly loading = signal(false);
  readonly login = this.credentials?.login ?? null;
  readonly form = this.fb.nonNullable.group({
    newPassword: ['', [Validators.required, Validators.minLength(12)]],
    confirmPassword: ['', Validators.required],
  });

  constructor() {
    if (!this.credentials) {
      void this.router.navigateByUrl('/login');
    }
  }

  submit(): void {
    if (!this.credentials) {
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { newPassword, confirmPassword } = this.form.getRawValue();
    if (newPassword !== confirmPassword) {
      this.form.controls.confirmPassword.setErrors({ mismatch: true });
      return;
    }

    const { login, currentPassword, twoFactorCode } = this.credentials;
    this.loading.set(true);
    this.auth
      .changePassword({ login, currentPassword, newPassword, confirmPassword, twoFactorCode })
      .subscribe({
        next: () => {
          this.loading.set(false);
          this.snackBar.open('Password changed. Please sign in with your new password.', 'OK', {
            duration: 4000,
          });
          void this.router.navigateByUrl('/login');
        },
        error: (error: HttpErrorResponse) => {
          this.loading.set(false);
          this.snackBar.open(error.error?.message ?? 'Could not change the password.', 'Dismiss', {
            duration: 6000,
          });
        },
      });
  }
}
