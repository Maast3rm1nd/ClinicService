export interface Schedule {
  id: string;
  doctor: string;
  appointments: string[];
}

export interface CreateScheduleRequest {
  doctor: string;
  appointments?: string[] | null;
}

export interface UpdateScheduleRequest {
  doctor?: string | null;
  appointments?: string[] | null;
}
