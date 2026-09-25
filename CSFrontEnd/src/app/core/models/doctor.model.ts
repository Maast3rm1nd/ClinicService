import { EmployeeWorkStatus } from './enums';

export interface Doctor {
  id: string;
  fullName: string;
  shortName?: string | null;
  login: string;
  specialisations: string[];
  employeeWorkStatus: EmployeeWorkStatus;
}

export interface CreateDoctorRequest {
  id: string;
  fullName: string;
  shortName?: string | null;
  login: string;
  specialisations: string[];
  employeeWorkStatus: EmployeeWorkStatus;
}

export interface UpdateDoctorRequest {
  fullName?: string | null;
  shortName?: string | null;
  login?: string | null;
  specialisations?: string[] | null;
  employeeWorkStatus?: EmployeeWorkStatus | null;
}
