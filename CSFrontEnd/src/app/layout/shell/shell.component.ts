import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { take } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { TwoFactorPromptDialogComponent } from '../../features/auth/security/two-factor-prompt-dialog.component';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MatSnackBarModule,
  ],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.css',
})
export class ShellComponent implements OnInit {
  protected readonly auth = inject(AuthService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly router = inject(Router);

  ngOnInit(): void {
    this.auth.getTwoFactorStatus().pipe(take(1)).subscribe({
      next: ({ enabled }) => {
        if (!enabled && !this.auth.hasSeenTwoFactorPrompt()) {
          this.auth.markTwoFactorPromptSeen();
          this.promptToEnableTwoFactor();
        }
      },
      error: () => {
        this.snackBar.open('Could not check your two-factor authentication status.', 'Dismiss', {
          duration: 6000,
        });
      },
    });
  }

  logout(): void {
    this.auth.logout();
  }

  private promptToEnableTwoFactor(): void {
    this.dialog
      .open(TwoFactorPromptDialogComponent, {
        width: '440px',
        maxWidth: 'calc(100vw - 32px)',
      })
      .afterClosed()
      .pipe(take(1))
      .subscribe((choice: 'setup' | 'skip' | undefined) => {
        if (choice === 'setup') {
          void this.router.navigateByUrl('/security', {
            state: { startTotpSetup: true },
          });
        }
      });
  }
}
