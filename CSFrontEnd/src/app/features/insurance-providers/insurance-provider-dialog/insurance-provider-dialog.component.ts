import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { HttpErrorResponse } from '@angular/common/http';
import { InsuranceProvidersService } from '../../../core/services/insurance-providers.service';
import { InsuranceProvider } from '../../../core/models/insurance-provider.model';

export interface InsuranceProviderDialogData {
  provider?: InsuranceProvider;
}

@Component({
  selector: 'app-insurance-provider-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './insurance-provider-dialog.component.html',
  styleUrl: './insurance-provider-dialog.component.css',
})
export class InsuranceProviderDialogComponent {
  protected readonly data = inject<InsuranceProviderDialogData>(MAT_DIALOG_DATA);
  private readonly fb = inject(FormBuilder);
  private readonly providersService = inject(InsuranceProvidersService);
  private readonly dialogRef = inject<MatDialogRef<InsuranceProviderDialogComponent, InsuranceProvider | undefined>>(
    MatDialogRef,
  );

  readonly saving = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly isEdit = !!this.data.provider;

  readonly form = this.fb.nonNullable.group({
    name: [this.data.provider?.name ?? '', Validators.required],
    licenseNumber: [this.data.provider?.licenseNumber ?? '', Validators.required],
    phoneNumber: [this.data.provider?.phoneNumber ?? ''],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, licenseNumber, phoneNumber } = this.form.getRawValue();
    this.saving.set(true);
    this.errorMessage.set(null);

    const request$ = this.isEdit
      ? this.providersService.update(this.data.provider!.id, {
          name,
          licenseNumber,
          phoneNumber: phoneNumber || null,
        })
      : this.providersService.create({ name, licenseNumber, phoneNumber: phoneNumber || null });

    request$.subscribe({
      next: (result) => {
        this.saving.set(false);
        this.dialogRef.close(result);
      },
      error: (error: HttpErrorResponse) => {
        this.saving.set(false);
        this.errorMessage.set(error.error?.message ?? 'Failed to save the insurance provider');
      },
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
