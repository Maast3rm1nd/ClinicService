import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import QRCode from 'qrcode';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-security',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  templateUrl: './security.component.html',
  styleUrl: './security.component.css',
})
export class SecurityComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly snackBar = inject(MatSnackBar);

  readonly loading = signal(true);
  readonly busy = signal(false);
  readonly enabled = signal(false);
  readonly secret = signal<string | null>(null);
  readonly qrCode = signal<string | null>(null);
  readonly codeForm = this.fb.nonNullable.group({
    code: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
    rememberDevice: [false],
  });

  ngOnInit(): void {
    this.loadStatus();
    if (history.state?.startTotpSetup) {
      this.startSetup();
    }
  }

  startSetup(): void {
    this.busy.set(true);
    this.auth.setupTwoFactor().subscribe({
      next: async ({ secret }) => {
        this.secret.set(secret);
        this.codeForm.reset({ code: '', rememberDevice: false });
        const login = this.auth.currentUser()?.login ?? 'user';
        const issuer = 'Clinic Service';
        const uri = `otpauth://totp/${encodeURIComponent(issuer)}:${encodeURIComponent(login)}?secret=${encodeURIComponent(secret)}&issuer=${encodeURIComponent(issuer)}&algorithm=SHA1&digits=6&period=30`;
        try {
          this.qrCode.set(await QRCode.toDataURL(uri, { width: 220, margin: 2 }));
        } catch {
          this.snackBar.open('Could not generate the authenticator QR code.', 'Dismiss', {
            duration: 5000,
          });
        }
        this.busy.set(false);
      },
      error: () => {
        this.busy.set(false);
        this.snackBar.open('Could not start two-factor setup.', 'Dismiss', { duration: 5000 });
      },
    });
  }

  confirmSetup(): void {
    if (this.codeForm.invalid) {
      this.codeForm.markAllAsTouched();
      return;
    }

    const { code, rememberDevice } = this.codeForm.getRawValue();
    this.busy.set(true);
    this.auth.confirmTwoFactor(code, rememberDevice).subscribe({
      next: () => {
        this.busy.set(false);
        this.enabled.set(true);
        this.secret.set(null);
        this.qrCode.set(null);
        this.codeForm.reset({ code: '', rememberDevice: false });
        this.snackBar.open('Two-factor authentication enabled.', 'OK', { duration: 4000 });
      },
      error: () => {
        this.busy.set(false);
        this.snackBar.open('The verification code was not accepted.', 'Dismiss', {
          duration: 5000,
        });
      },
    });
  }

  disable(): void {
    if (this.codeForm.controls.code.invalid) {
      this.codeForm.controls.code.markAsTouched();
      return;
    }

    this.busy.set(true);
    this.auth.disableTwoFactor(this.codeForm.controls.code.value).subscribe({
      next: () => {
        this.busy.set(false);
        this.enabled.set(false);
        this.codeForm.reset({ code: '', rememberDevice: false });
        this.snackBar.open('Two-factor authentication disabled.', 'OK', { duration: 4000 });
      },
      error: () => {
        this.busy.set(false);
        this.snackBar.open(
          'Could not disable two-factor authentication. Check the code.',
          'Dismiss',
          {
            duration: 5000,
          },
        );
      },
    });
  }

  private loadStatus(): void {
    this.auth.getTwoFactorStatus().subscribe({
      next: ({ enabled }) => {
        this.enabled.set(enabled);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Could not load two-factor status.', 'Dismiss', { duration: 5000 });
      },
    });
  }
}
