import { SnapshotBase } from './person.model';
import { DiagnosisStatus } from './enums';

export interface CardNote extends SnapshotBase {
  medicalCard: string;
  doctor: string;
  appointment?: string | null;
  dateNote: string;
  patientsComplaints?: string | null;
  diagnoses?: string[] | null;
  creationDateTime: string;
  editDateTime?: string | null;
}

export interface NewCardNoteDiagnosis {
  icdCode: string;
  name?: string | null;
  status?: DiagnosisStatus | null;
}

export interface CreateCardNoteRequest {
  medicalCard: string;
  doctor: string;
  appointment?: string | null;
  dateNote: string;
  patientsComplaints?: string | null;
  diagnoses?: string[] | null;
  newDiagnoses?: NewCardNoteDiagnosis[] | null;
}
