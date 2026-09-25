import { Injectable } from '@angular/core';
import { TokenResponse } from '../models/auth.model';

const ACCESS_TOKEN_KEY = 'clinic.accessToken';
const REFRESH_TOKEN_KEY = 'clinic.refreshToken';
const REFRESH_EXPIRES_KEY = 'clinic.refreshTokenExpiresAt';

/** Wraps localStorage access for the JWT access/refresh token pair. */
@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  save(tokens: TokenResponse): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, tokens.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);
    localStorage.setItem(REFRESH_EXPIRES_KEY, tokens.refreshTokenExpiresAt);
  }

  get accessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  get refreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  clear(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(REFRESH_EXPIRES_KEY);
  }
}
