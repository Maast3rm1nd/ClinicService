import { Component, OnInit, inject, signal } from '@angular/core';
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

@Component({
  selector: 'app-set-password',
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
  templateUrl: './set-password.component.html',
  styleUrl: './set-password.component.css',
})
export class SetPasswordComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  readonly loading = signal(false);
  readonly validationState = signal<'checking' | 'valid' | 'invalid'>('checking');
  readonly login = signal<string | null>(null);
  readonly token = this.readTokenFromFragment();
  readonly form = this.fb.nonNullable.group({
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', Validators.required],
  });

  ngOnInit(): void {
    if (!this.token) {
      this.validationState.set('invalid');
      return;
    }

    this.auth.validatePasswordSetup(this.token).subscribe({
      next: ({ login }) => {
        this.login.set(login);
        this.validationState.set('valid');
      },
      error: () => this.validationState.set('invalid'),
    });
  }

  submit(): void {
    if (!this.token || this.validationState() !== 'valid') {
      this.snackBar.open('This password setup link is missing or invalid.', 'Dismiss', {
        duration: 5000,
      });
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { password, confirmPassword } = this.form.getRawValue();
    if (password !== confirmPassword) {
      this.form.controls.confirmPassword.setErrors({ mismatch: true });
      return;
    }

    this.loading.set(true);
    this.auth.setPassword({ token: this.token, password }).subscribe({
      next: () => {
        this.loading.set(false);
        this.snackBar.open('Password set. You can now sign in.', 'OK', { duration: 4000 });
        void this.router.navigateByUrl('/login');
      },
      error: (error: HttpErrorResponse) => {
        this.loading.set(false);
        this.validationState.set('invalid');
        this.snackBar.open(
          error.error?.message ?? 'Could not set the password. The link may have expired.',
          'Dismiss',
          {
            duration: 6000,
          },
        );
      },
    });
  }

  private readTokenFromFragment(): string {
    const fragment = window.location.hash.replace(/^#/, '');
    return new URLSearchParams(fragment).get('token') ?? '';
  }
}
