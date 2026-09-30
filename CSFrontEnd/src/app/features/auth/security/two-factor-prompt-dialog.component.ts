import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-two-factor-prompt-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <h2 mat-dialog-title>Protect your account</h2>
    <mat-dialog-content>
      <p>Two-factor authentication adds an extra layer of security using an authenticator app.</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button type="button" (click)="close('skip')">Not now</button>
      <button mat-flat-button color="primary" type="button" (click)="close('setup')">
        <mat-icon>security</mat-icon>
        Set up now
      </button>
    </mat-dialog-actions>
  `,
})
export class TwoFactorPromptDialogComponent {
  constructor(private readonly dialogRef: MatDialogRef<TwoFactorPromptDialogComponent>) {}

  close(choice: 'setup' | 'skip'): void {
    this.dialogRef.close(choice);
  }
}
