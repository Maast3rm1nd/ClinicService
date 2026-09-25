import { SnapshotBase } from './person.model';

export interface MedicalCard extends SnapshotBase {
  patient: string;
  recordNumber: number;
  policy?: string | null;
  diagnoses?: string[] | null;
  creationDateTime: string;
}
