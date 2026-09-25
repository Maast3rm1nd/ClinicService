import { SnapshotBase } from './person.model';
import { AppointmentStatus } from './enums';

export interface Appointment extends SnapshotBase {
  patient: string;
  medicalCard: string;
  doctor: string;
  appointmentDateTime: string;
  createdBy: string;
  editedBy?: string | null;
  status: AppointmentStatus;
  preliminaryReason?: string | null;
  creationDateTime: string;
  editDateTime?: string | null;
}

export interface CreateAppointmentRequest {
  patient: string;
  medicalCard: string;
  doctor: string;
  appointmentDateTime: string;
  createdBy: string;
  preliminaryReason?: string | null;
}

export interface UpdateAppointmentRequest {
  appointmentDateTime?: string | null;
  editedBy: string;
  status?: AppointmentStatus | null;
  preliminaryReason?: string | null;
}
