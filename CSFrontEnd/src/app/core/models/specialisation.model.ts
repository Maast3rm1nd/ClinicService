import { SnapshotBase } from './person.model';

export interface Specialisation extends SnapshotBase {
  name: string;
  doctors?: string[] | null;
}

export interface CreateSpecialisationRequest {
  name: string;
}

export interface UpdateSpecialisationRequest {
  name?: string | null;
}
