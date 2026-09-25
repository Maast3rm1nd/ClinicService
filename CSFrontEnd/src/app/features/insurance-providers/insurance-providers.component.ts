import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { InsuranceProvidersService } from '../../core/services/insurance-providers.service';
import { InsuranceProvider } from '../../core/models/insurance-provider.model';
import { InsuranceProviderDialogComponent } from './insurance-provider-dialog/insurance-provider-dialog.component';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-insurance-providers',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './insurance-providers.component.html',
  styleUrl: './insurance-providers.component.css',
})
export class InsuranceProvidersComponent {
  readonly loading = signal(true);
  readonly providers = signal<InsuranceProvider[]>([]);
  readonly displayedColumns = ['name', 'license', 'phone', 'actions'];

  constructor(
    private readonly providersService: InsuranceProvidersService,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
    protected readonly auth: AuthService,
  ) {
    this.load();
  }

  openCreate(): void {
    this.dialog
      .open(InsuranceProviderDialogComponent, { width: '420px', data: {} })
      .afterClosed()
      .subscribe((created) => {
        if (created) {
          this.providers.update((list) => [...list, created]);
        }
      });
  }

  edit(provider: InsuranceProvider): void {
    this.dialog
      .open(InsuranceProviderDialogComponent, { width: '420px', data: { provider } })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.providers.update((list) => list.map((p) => (p.id === updated.id ? updated : p)));
        }
      });
  }

  remove(provider: InsuranceProvider): void {
    if (!confirm(`Delete insurance provider "${provider.name}"?`)) {
      return;
    }

    this.providersService.delete(provider.id).subscribe({
      next: () => {
        this.providers.update((list) => list.filter((p) => p.id !== provider.id));
        this.snackBar.open('Insurance provider deleted', 'OK', { duration: 3000 });
      },
      error: () => this.snackBar.open('Failed to delete the record', 'OK', { duration: 3000 }),
    });
  }

  private load(): void {
    this.loading.set(true);
    this.providersService.getAll().subscribe({
      next: (items) => {
        this.providers.set(items);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
