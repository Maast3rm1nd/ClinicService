import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

export interface TwoFactorSecretDialogData {
  doctorName: string;
  login: string;
  secret: string;
}

@Component({
  selector: 'app-two-factor-secret-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Two-factor authentication enabled</h2>
    <mat-dialog-content>
      <p>
        Give this key to <strong>{{ data.doctorName }}</strong> ({{ data.login }}) to add it to an
        authenticator app. It is shown only once.
      </p>
      <code>{{ data.secret }}</code>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-flat-button color="primary" type="button" mat-dialog-close>Done</button>
    </mat-dialog-actions>
  `,
  styles: `
    code {
      display: block;
      padding: 0.75rem;
      background: #f5f5f5;
      border-radius: 4px;
      word-break: break-all;
      user-select: all;
    }
  `,
})
export class TwoFactorSecretDialogComponent {
  protected readonly data = inject<TwoFactorSecretDialogData>(MAT_DIALOG_DATA);
}
