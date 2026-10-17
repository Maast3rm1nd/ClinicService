export interface SetPasswordRequest {
  token: string;
  password: string;
}

export interface PasswordSetupValidationResponse {
  login: string;
}
