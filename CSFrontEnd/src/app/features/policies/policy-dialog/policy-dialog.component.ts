import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { HttpErrorResponse } from '@angular/common/http';
import { PoliciesService } from '../../../core/services/policies.service';
import { Policy } from '../../../core/models/policy.model';
import { InsuranceProvider } from '../../../core/models/insurance-provider.model';
import { PolicyType, policyTypeLabels } from '../../../core/models/enums';

export interface PolicyDialogData {
  policy?: Policy;
  providers: InsuranceProvider[];
}

@Component({
  selector: 'app-policy-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
  ],
  templateUrl: './policy-dialog.component.html',
  styleUrl: './policy-dialog.component.css',
})
export class PolicyDialogComponent {
  protected readonly data = inject<PolicyDialogData>(MAT_DIALOG_DATA);
  private readonly fb = inject(FormBuilder);
  private readonly policiesService = inject(PoliciesService);
  private readonly dialogRef = inject<MatDialogRef<PolicyDialogComponent, Policy | undefined>>(MatDialogRef);

  readonly saving = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly policyTypes = Object.values(PolicyType);
  readonly policyTypeLabels = policyTypeLabels;
  readonly isEdit = !!this.data.policy;

  readonly form = this.fb.nonNullable.group({
    medicalPolicyNumber: [this.data.policy?.medicalPolicyNumber ?? '', Validators.required],
    medicalPolicyType: [this.data.policy?.medicalPolicyType ?? PolicyType.EHIC],
    insuranceProvider: [this.data.policy?.insuranceProvider ?? '', Validators.required],
    description: [this.data.policy?.description ?? ''],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { medicalPolicyNumber, medicalPolicyType, insuranceProvider, description } = this.form.getRawValue();
    this.saving.set(true);
    this.errorMessage.set(null);

    const request$ = this.isEdit
      ? this.policiesService.update(this.data.policy!.id, {
          medicalPolicyNumber,
          medicalPolicyType,
          insuranceProvider,
          description: description || null,
        })
      : this.policiesService.create({
          medicalPolicyNumber,
          medicalPolicyType,
          insuranceProvider,
          description: description || null,
        });

    request$.subscribe({
      next: (result) => {
        this.saving.set(false);
        this.dialogRef.close(result);
      },
      error: (error: HttpErrorResponse) => {
        this.saving.set(false);
        this.errorMessage.set(error.error?.message ?? 'Failed to save the policy');
      },
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
