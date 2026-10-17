export interface LoginRequest {
  login: string;
  password: string;
  twoFactorCode?: string | null;
}

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  tokenType: string;
  expiresIn: number;
  refreshTokenExpiresAt: string;
  twoFactorRequired?: boolean;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface LogoutRequest {
  refreshToken: string;
}

export interface TwoFactorStatusResponse {
  enabled: boolean;
}

export interface TwoFactorSetupResponse {
  secret: string;
}

export interface ChangePasswordRequest {
  login: string;
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
  twoFactorCode?: string | null;
}

export interface CurrentUser {
  id: string;
  login: string;
  role: 'Administrator' | 'Doctor' | string;
  permissions: string[];
}
