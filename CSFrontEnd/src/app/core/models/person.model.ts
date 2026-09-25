import { PersonType } from './enums';

export interface SnapshotBase {
  id: string;
  entityId: string;
  version: number;
  validFrom: string;
  validTo?: string | null;
}

export interface Person extends SnapshotBase {
  fullName: string;
  shortName?: string | null;
  login: string;
  type: PersonType;
  creationDateTime: string;
  editDateTime?: string | null;
}

export interface CreatePersonRequest {
  fullName: string;
  shortName?: string | null;
  login: string;
  password: string;
}

export interface UpdatePersonRequest {
  fullName?: string | null;
  shortName?: string | null;
  login?: string | null;
}
