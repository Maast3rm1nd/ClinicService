import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PoliciesService } from '../../core/services/policies.service';
import { InsuranceProvidersService } from '../../core/services/insurance-providers.service';
import { Policy } from '../../core/models/policy.model';
import { InsuranceProvider } from '../../core/models/insurance-provider.model';
import { policyTypeLabels } from '../../core/models/enums';
import { PolicyDialogComponent } from './policy-dialog/policy-dialog.component';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-policies',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './policies.component.html',
  styleUrl: './policies.component.css',
})
export class PoliciesComponent {
  readonly loading = signal(true);
  readonly policies = signal<Policy[]>([]);
  readonly providers = signal<InsuranceProvider[]>([]);
  readonly policyTypeLabels = policyTypeLabels;
  readonly displayedColumns = ['number', 'type', 'provider', 'description', 'actions'];

  constructor(
    private readonly policiesService: PoliciesService,
    private readonly providersService: InsuranceProvidersService,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
    protected readonly auth: AuthService,
  ) {
    this.load();
  }

  providerName(policy: Policy): string {
    return this.providers().find((p) => p.entityId === policy.insuranceProvider)?.name ?? '—';
  }

  policyTypeLabel(policy: Policy): string {
    return policy.medicalPolicyType ? this.policyTypeLabels[policy.medicalPolicyType] : '—';
  }

  openCreate(): void {
    this.dialog
      .open(PolicyDialogComponent, { width: '480px', data: { providers: this.providers() } })
      .afterClosed()
      .subscribe((created) => {
        if (created) {
          this.policies.update((list) => [...list, created]);
        }
      });
  }

  edit(policy: Policy): void {
    this.dialog
      .open(PolicyDialogComponent, { width: '480px', data: { policy, providers: this.providers() } })
      .afterClosed()
      .subscribe((updated) => {
        if (updated) {
          this.policies.update((list) => list.map((p) => (p.id === updated.id ? updated : p)));
        }
      });
  }

  remove(policy: Policy): void {
    if (!confirm(`Delete policy "${policy.medicalPolicyNumber}"?`)) {
      return;
    }

    this.policiesService.delete(policy.id).subscribe({
      next: () => {
        this.policies.update((list) => list.filter((p) => p.id !== policy.id));
        this.snackBar.open('Policy deleted', 'OK', { duration: 3000 });
      },
      error: () => this.snackBar.open('Failed to delete the policy', 'OK', { duration: 3000 }),
    });
  }

  private load(): void {
    this.loading.set(true);
    this.providersService.getAll().subscribe((items) => this.providers.set(items));
    this.policiesService.getAll().subscribe({
      next: (items) => {
        this.policies.set(items);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
