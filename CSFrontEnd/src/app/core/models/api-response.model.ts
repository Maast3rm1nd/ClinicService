export interface ListResponse<T> {
  data: T[];
}

export interface ErrorResponse {
  code: number;
  message: string;
  details?: string | null;
}
