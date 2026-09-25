import { SnapshotBase } from './person.model';
import { PolicyType } from './enums';

export interface Policy extends SnapshotBase {
  medicalPolicyNumber: string;
  medicalPolicyType?: PolicyType | null;
  insuranceProvider: string;
  description?: string | null;
  creationDateTime: string;
}

export interface CreatePolicyRequest {
  medicalPolicyNumber: string;
  medicalPolicyType?: PolicyType | null;
  insuranceProvider: string;
  description?: string | null;
}

export interface UpdatePolicyRequest {
  medicalPolicyNumber?: string | null;
  medicalPolicyType?: PolicyType | null;
  insuranceProvider?: string | null;
  description?: string | null;
}
