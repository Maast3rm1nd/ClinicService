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

export interface CurrentUser {
  login: string;
  role: 'Administrator' | 'Doctor' | string;
  permissions: string[];
}
