import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { forkJoin } from 'rxjs';
import { AppointmentsService } from '../../core/services/appointments.service';
import { DoctorsService } from '../../core/services/doctors.service';
import { PatientsService } from '../../core/services/patients.service';
import { MedicalCardsService } from '../../core/services/medical-cards.service';
import { Appointment } from '../../core/models/appointment.model';
import { Doctor } from '../../core/models/doctor.model';
import { Patient } from '../../core/models/patient.model';
import { MedicalCard } from '../../core/models/medical-card.model';
import { AppointmentStatus, appointmentStatusLabels } from '../../core/models/enums';
import { AppointmentDialogComponent, AppointmentDialogData } from './appointment-dialog/appointment-dialog.component';

interface CalendarEvent {
  appointment: Appointment;
  dayIndex: number;
  topPx: number;
  heightPx: number;
  doctorName: string;
  patientName: string;
}

const START_HOUR = 7;
const END_HOUR = 21;
const HOUR_HEIGHT_PX = 64;
const DEFAULT_DURATION_MIN = 30;
const DAY_LABELS = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent {
  readonly loading = signal(true);
  readonly weekStart = signal(this.mondayOf(new Date()));
  readonly selectedDoctorId = signal<string>('all');

  readonly doctors = signal<Doctor[]>([]);
  readonly patients = signal<Patient[]>([]);
  readonly medicalCards = signal<MedicalCard[]>([]);
  readonly appointments = signal<Appointment[]>([]);

  readonly hours = Array.from({ length: END_HOUR - START_HOUR }, (_, i) => START_HOUR + i);
  readonly dayLabels = DAY_LABELS;
  readonly statusLabels = appointmentStatusLabels;
  readonly hourHeight = HOUR_HEIGHT_PX;

  readonly weekDays = computed(() => {
    const start = this.weekStart();
    return Array.from({ length: 7 }, (_, i) => {
      const d = new Date(start);
      d.setDate(d.getDate() + i);
      return d;
    });
  });

  readonly weekRangeLabel = computed(() => {
    const days = this.weekDays();
    const first = days[0];
    const last = days[6];
    const fmt = (d: Date) => d.toLocaleDateString('en-US', { day: 'numeric', month: 'long' });
    return `${fmt(first)} – ${fmt(last)} ${last.getFullYear()}`;
  });

  private readonly doctorNameById = computed(() => {
    const map = new Map<string, string>();
    for (const doctor of this.doctors()) {
      map.set(doctor.id, doctor.shortName || doctor.fullName);
    }
    return map;
  });

  private readonly patientNameById = computed(() => {
    const map = new Map<string, string>();
    for (const patient of this.patients()) {
      map.set(patient.entityId, patient.shortName || patient.fullName);
    }
    return map;
  });

  readonly events = computed<CalendarEvent[]>(() => {
    const days = this.weekDays();
    const weekStartMs = days[0].setHours(0, 0, 0, 0);
    const weekEndMs = new Date(days[6]).setHours(23, 59, 59, 999);
    const doctorFilter = this.selectedDoctorId();
    const doctorNames = this.doctorNameById();
    const patientNames = this.patientNameById();

    return this.appointments()
      .filter((a) => a.status !== AppointmentStatus.Cancelled)
      .filter((a) => doctorFilter === 'all' || a.doctor === doctorFilter)
      .map((a) => {
        const date = new Date(a.appointmentDateTime);
        return { appointment: a, date };
      })
      .filter(({ date }) => date.getTime() >= weekStartMs && date.getTime() <= weekEndMs)
      .map(({ appointment, date }) => {
        const dayIndex = (date.getDay() + 6) % 7; // Monday = 0
        const minutesFromStart = (date.getHours() - START_HOUR) * 60 + date.getMinutes();
        return {
          appointment,
          dayIndex,
          topPx: (minutesFromStart / 60) * HOUR_HEIGHT_PX,
          heightPx: (DEFAULT_DURATION_MIN / 60) * HOUR_HEIGHT_PX,
          doctorName: doctorNames.get(appointment.doctor) ?? 'Unknown doctor',
          patientName: patientNames.get(appointment.patient) ?? 'Patient',
        } satisfies CalendarEvent;
      });
  });

  constructor(
    private readonly appointmentsService: AppointmentsService,
    private readonly doctorsService: DoctorsService,
    private readonly patientsService: PatientsService,
    private readonly medicalCardsService: MedicalCardsService,
    private readonly dialog: MatDialog,
  ) {
    this.loadAll();
  }

  private loadAll(): void {
    this.loading.set(true);
    forkJoin({
      doctors: this.doctorsService.getAll(),
      patients: this.patientsService.getAll(),
      medicalCards: this.medicalCardsService.getAll(),
      appointments: this.appointmentsService.getAll(),
    }).subscribe({
      next: ({ doctors, patients, medicalCards, appointments }) => {
        this.doctors.set(doctors);
        this.patients.set(patients);
        this.medicalCards.set(medicalCards);
        this.appointments.set(appointments);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  previousWeek(): void {
    const d = new Date(this.weekStart());
    d.setDate(d.getDate() - 7);
    this.weekStart.set(d);
  }

  nextWeek(): void {
    const d = new Date(this.weekStart());
    d.setDate(d.getDate() + 7);
    this.weekStart.set(d);
  }

  today(): void {
    this.weekStart.set(this.mondayOf(new Date()));
  }

  openNewAppointment(day?: Date, hour?: number): void {
    const initialDate = day ? new Date(day) : new Date();
    if (hour !== undefined) {
      initialDate.setHours(hour, 0, 0, 0);
    }

    const data: AppointmentDialogData = {
      doctors: this.doctors(),
      patients: this.patients(),
      medicalCards: this.medicalCards(),
      initialDate,
    };

    this.dialog
      .open(AppointmentDialogComponent, { width: '480px', data })
      .afterClosed()
      .subscribe((created) => {
        if (created) {
          this.appointments.update((list) => [...list, created]);
        }
      });
  }

  private mondayOf(date: Date): Date {
    const d = new Date(date);
    const day = (d.getDay() + 6) % 7;
    d.setDate(d.getDate() - day);
    d.setHours(0, 0, 0, 0);
    return d;
  }
}
