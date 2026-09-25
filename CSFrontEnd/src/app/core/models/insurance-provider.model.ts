import { SnapshotBase } from './person.model';

export interface InsuranceProvider extends SnapshotBase {
  name: string;
  licenseNumber: string;
  phoneNumber?: string | null;
  creationDateTime: string;
}

export interface CreateInsuranceProviderRequest {
  name: string;
  licenseNumber: string;
  phoneNumber?: string | null;
}

export interface UpdateInsuranceProviderRequest {
  name?: string | null;
  licenseNumber?: string | null;
  phoneNumber?: string | null;
}
