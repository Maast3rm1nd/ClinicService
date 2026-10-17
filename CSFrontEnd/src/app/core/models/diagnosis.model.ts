import { SnapshotBase } from './person.model';
import { DiagnosisStatus } from './enums';

export interface Diagnosis extends SnapshotBase {
  icdCode: string;
  name?: string | null;
  doctor: string;
  editedDoctor?: string | null;
  medicalCard: string;
  status: DiagnosisStatus;
  creationDateTime: string;
  editDateTime?: string | null;
}

/** Formats a diagnosis as "code - name" (just the code when the name is unknown). */
export function diagnosisLabel(diagnosis: Pick<Diagnosis, 'icdCode' | 'name'>): string {
  return diagnosis.name ? `${diagnosis.icdCode} - ${diagnosis.name}` : diagnosis.icdCode;
}
