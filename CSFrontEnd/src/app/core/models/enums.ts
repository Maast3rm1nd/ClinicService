// Mirrors ClinicServiceContext.Enums.* on the backend (serialized as camelCase strings).

export enum PersonType {
  Person = 'person',
  Administrator = 'administrator',
  Doctor = 'doctor',
}

export enum EmployeeWorkStatus {
  AtWork = 'atWork',
  SickLeave = 'sickLeave',
  Vacation = 'vacation',
}

export enum AppointmentStatus {
  Pending = 'pending',
  Confirmed = 'confirmed',
  Cancelled = 'cancelled',
  Completed = 'completed',
  Rescheduled = 'rescheduled',
}

export enum PolicyType {
  EHIC = 'ehic',
  Private = 'private',
  Mixed = 'mixed',
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
};

export const policyTypeLabels: Record<PolicyType, string> = {
  [PolicyType.EHIC]: 'EHIC (mandatory)',
  [PolicyType.Private]: 'Private (voluntary)',
  [PolicyType.Mixed]: 'Mixed',
};
