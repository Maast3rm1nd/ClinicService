import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { HttpErrorResponse } from '@angular/common/http';
import { AppointmentsService } from '../../../core/services/appointments.service';
import { PersonsService } from '../../../core/services/persons.service';
import { AuthService } from '../../../core/services/auth.service';
import { Doctor } from '../../../core/models/doctor.model';
import { Patient } from '../../../core/models/patient.model';
import { MedicalCard } from '../../../core/models/medical-card.model';
import { Appointment } from '../../../core/models/appointment.model';

export interface AppointmentDialogData {
  doctors: Doctor[];
  patients: Patient[];
  medicalCards: MedicalCard[];
  initialDate: Date;
}

@Component({
  selector: 'app-appointment-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
  templateUrl: './appointment-dialog.component.html',
  styleUrl: './appointment-dialog.component.css',
})
export class AppointmentDialogComponent {
  protected readonly data = inject<AppointmentDialogData>(MAT_DIALOG_DATA);
  private readonly fb = inject(FormBuilder);
  private readonly appointmentsService = inject(AppointmentsService);
  private readonly personsService = inject(PersonsService);
  private readonly authService = inject(AuthService);
  private readonly dialogRef = inject<MatDialogRef<AppointmentDialogComponent, Appointment | undefined>>(
    MatDialogRef,
  );

  readonly saving = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    doctor: ['', Validators.required],
    patient: ['', Validators.required],
    date: [this.data.initialDate, Validators.required],
    time: [this.formatTime(this.data.initialDate), Validators.required],
    preliminaryReason: [''],
  });

  cardsForSelectedPatient(): MedicalCard[] {
    const patientId = this.form.controls.patient.value;
    return this.data.medicalCards.filter((card) => card.patient === patientId);
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { doctor, patient, date, time, preliminaryReason } = this.form.getRawValue();
    const medicalCard = this.cardsForSelectedPatient()[0]?.entityId;

    if (!medicalCard) {
      this.errorMessage.set('The selected patient has no medical card — cannot create an appointment');
      return;
    }

    const [hours, minutes] = time.split(':').map(Number);
    const appointmentDate = new Date(date);
    appointmentDate.setHours(hours, minutes, 0, 0);

    this.saving.set(true);
    this.errorMessage.set(null);

    this.personsService.getAll().subscribe({
      next: (persons) => {
        const currentLogin = this.authService.currentUser()?.login;
        const createdBy = persons.find((p) => p.login === currentLogin)?.entityId ?? persons[0]?.entityId;

        if (!createdBy) {
          this.saving.set(false);
          this.errorMessage.set('Could not determine the current user');
          return;
        }

        this.appointmentsService
          .create({
            patient,
            medicalCard,
            doctor,
            appointmentDateTime: appointmentDate.toISOString(),
            createdBy,
            preliminaryReason: preliminaryReason || null,
          })
          .subscribe({
            next: (created) => {
              this.saving.set(false);
              this.dialogRef.close(created);
            },
            error: (error: HttpErrorResponse) => {
              this.saving.set(false);
              this.errorMessage.set(error.error?.message ?? 'Failed to create the appointment');
            },
          });
      },
      error: () => {
        this.saving.set(false);
        this.errorMessage.set('Could not determine the current user');
      },
    });
  }

  cancel(): void {
    this.dialogRef.close();
  }

  private formatTime(date: Date): string {
    return `${date.getHours().toString().padStart(2, '0')}:${date.getMinutes().toString().padStart(2, '0')}`;
  }
}
