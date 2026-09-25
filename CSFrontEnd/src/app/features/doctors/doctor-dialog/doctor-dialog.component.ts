import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { HttpErrorResponse } from '@angular/common/http';
import { DoctorsService } from '../../../core/services/doctors.service';
import { Doctor } from '../../../core/models/doctor.model';
import { Specialisation } from '../../../core/models/specialisation.model';
import { EmployeeWorkStatus, employeeWorkStatusLabels } from '../../../core/models/enums';

export interface DoctorDialogData {
  doctor: Doctor;
  specialisations: Specialisation[];
}

@Component({
  selector: 'app-doctor-dialog',
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
  templateUrl: './doctor-dialog.component.html',
  styleUrl: './doctor-dialog.component.css',
})
export class DoctorDialogComponent {
  protected readonly data = inject<DoctorDialogData>(MAT_DIALOG_DATA);
  private readonly fb = inject(FormBuilder);
  private readonly doctorsService = inject(DoctorsService);
  private readonly dialogRef = inject<MatDialogRef<DoctorDialogComponent, Doctor | undefined>>(MatDialogRef);

  readonly saving = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly workStatuses = Object.values(EmployeeWorkStatus);
  readonly workStatusLabels = employeeWorkStatusLabels;

  readonly form = this.fb.nonNullable.group({
    fullName: [this.data.doctor.fullName, Validators.required],
    shortName: [this.data.doctor.shortName ?? ''],
    login: [this.data.doctor.login, Validators.required],
    specialisations: this.fb.nonNullable.control<string[]>([...this.data.doctor.specialisations]),
    employeeWorkStatus: [this.data.doctor.employeeWorkStatus, Validators.required],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.errorMessage.set(null);

    const { fullName, shortName, login, specialisations, employeeWorkStatus } = this.form.getRawValue();

    this.doctorsService
      .update(this.data.doctor.id, {
        fullName,
        shortName: shortName || null,
        login,
        specialisations,
        employeeWorkStatus,
      })
      .subscribe({
        next: (updated) => {
          this.saving.set(false);
          this.dialogRef.close(updated);
        },
        error: (error: HttpErrorResponse) => {
          this.saving.set(false);
          this.errorMessage.set(error.error?.message ?? 'Failed to update doctor information');
        },
      });
  }

  cancel(): void {
    this.dialogRef.close();
  }
}
