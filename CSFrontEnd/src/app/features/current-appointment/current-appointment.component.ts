import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { forkJoin } from 'rxjs';
import { AppointmentsService } from '../../core/services/appointments.service';
import { AuthService } from '../../core/services/auth.service';
import { PatientsService } from '../../core/services/patients.service';
import { Appointment } from '../../core/models/appointment.model';
import { Patient } from '../../core/models/patient.model';
import { AppointmentStatus, appointmentStatusLabels } from '../../core/models/enums';

/** Appointments that started up to this long ago are still considered "current". */
const CURRENT_WINDOW_MS = 30 * 60 * 1000;

@Component({
  selector: 'app-current-appointment',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './current-appointment.component.html',
  styleUrl: './current-appointment.component.css',
})
export class CurrentAppointmentComponent {
  private readonly appointmentsService = inject(AppointmentsService);
  private readonly patientsService = inject(PatientsService);
  private readonly auth = inject(AuthService);
  private readonly snackBar = inject(MatSnackBar);

  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly appointment = signal<Appointment | null>(null);
  readonly patients = signal<Patient[]>([]);
  readonly statusLabels = appointmentStatusLabels;
  readonly AppointmentStatus = AppointmentStatus;

  readonly patientName = computed(() => {
    const appointment = this.appointment();
    const patient = this.patients().find((p) => p.entityId === appointment?.patient);
    return patient?.fullName ?? appointment?.patient ?? '';
  });

  constructor() {
    this.load();
  }

  start(): void {
    this.setStatus(AppointmentStatus.InProgress);
  }

  complete(): void {
    this.setStatus(AppointmentStatus.Completed);
  }

  private setStatus(status: AppointmentStatus): void {
    const appointment = this.appointment();
    if (!appointment) {
      return;
    }

    this.saving.set(true);
    this.appointmentsService
      .update(appointment.id, { editedBy: this.auth.currentUser()?.id ?? '', status })
      .subscribe({
        next: () => {
          this.saving.set(false);
          this.load();
        },
        error: () => {
          this.saving.set(false);
          this.snackBar.open('Failed to update the appointment', 'OK', { duration: 4000 });
        },
      });
  }

  private load(): void {
    const doctorId = this.auth.currentUser()?.id;
    if (!doctorId) {
      this.loading.set(false);
      return;
    }

    this.loading.set(true);
    forkJoin({
      appointments: this.appointmentsService.filter(
        [
          ['doctor', 'Equal', doctorId],
          'And',
          [
            'status',
            'AnyOf',
            [AppointmentStatus.InProgress, AppointmentStatus.Confirmed, AppointmentStatus.Pending],
          ],
        ],
        { sortBy: 'appointmentDateTime' },
      ),
      patients: this.patientsService.getAll(),
    }).subscribe({
      next: ({ appointments, patients }) => {
        this.patients.set(patients);
        this.appointment.set(this.pickCurrent(appointments));
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load the current appointment', 'OK', { duration: 4000 });
      },
    });
  }

  /** The appointment already in progress, otherwise the nearest one that is not long past. */
  private pickCurrent(appointments: Appointment[]): Appointment | null {
    const inProgress = appointments.find((a) => a.status === AppointmentStatus.InProgress);
    if (inProgress) {
      return inProgress;
    }

    const threshold = Date.now() - CURRENT_WINDOW_MS;
    return appointments.find((a) => new Date(a.appointmentDateTime).getTime() >= threshold) ?? null;
  }
}
