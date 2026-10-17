// Mirrors ClinicServiceContext.Enums.* on the backend (serialized as PascalCase strings).

export enum PersonType {
  Person = 'Person',
  Administrator = 'Administrator',
  Doctor = 'Doctor',
}

export enum EmployeeWorkStatus {
  AtWork = 'AtWork',
  SickLeave = 'SickLeave',
  Vacation = 'Vacation',
}

export enum AppointmentStatus {
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Cancelled = 'Cancelled',
  Completed = 'Completed',
  Rescheduled = 'Rescheduled',
  InProgress = 'InProgress',
}

export enum PolicyType {
  EHIC = 'EHIC',
  Private = 'Private',
  Mixed = 'Mixed',
}

export const employeeWorkStatusLabels: Record<EmployeeWorkStatus, string> = {
  [EmployeeWorkStatus.AtWork]: 'At work',
  [EmployeeWorkStatus.SickLeave]: 'Sick leave',
  [EmployeeWorkStatus.Vacation]: 'Vacation',
};

export const appointmentStatusLabels: Record<AppointmentStatus, string> = {
  [AppointmentStatus.Pending]: 'Pending',
  [AppointmentStatus.Confirmed]: 'Confirmed',
  [AppointmentStatus.Cancelled]: 'Cancelled',
  [AppointmentStatus.Completed]: 'Completed',
  [AppointmentStatus.Rescheduled]: 'Rescheduled',
  [AppointmentStatus.InProgress]: 'In progress',
};

export const policyTypeLabels: Record<PolicyType, string> = {
  [PolicyType.EHIC]: 'EHIC (mandatory)',
  [PolicyType.Private]: 'Private (voluntary)',
  [PolicyType.Mixed]: 'Mixed',
};

export enum DiagnosisStatus {
  Actual = 'Actual',
  Inactual = 'Inactual',
}

export const diagnosisStatusLabels: Record<DiagnosisStatus, string> = {
  [DiagnosisStatus.Actual]: 'Actual',
  [DiagnosisStatus.Inactual]: 'Inactual',
};
