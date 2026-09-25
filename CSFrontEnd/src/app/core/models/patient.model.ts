import { SnapshotBase } from './person.model';

export interface Patient extends SnapshotBase {
  fullName: string;
  shortName?: string | null;
  passportNumber?: string | null;
  birthDate: string;
  phoneNumber?: string | null;
  bloodGroup?: string | null;
  allergies?: string | null;
  creationDateTime: string;
  editDateTime?: string | null;
}
