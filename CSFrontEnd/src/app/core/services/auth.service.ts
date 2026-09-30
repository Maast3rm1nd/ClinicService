import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CurrentUser,
  LoginRequest,
  LogoutRequest,
  RefreshTokenRequest,
  TokenResponse,
  TwoFactorSetupResponse,
  TwoFactorStatusResponse,
} from '../models/auth.model';
import { decodeJwtPayload, readClaim, readClaimArray } from './jwt.util';
import { TokenStorageService } from './token-storage.service';

/** Handles login/refresh/logout and exposes the current authenticated user as a signal. */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private static readonly twoFactorPromptStorageKey = 'clinic.two-factor-prompted';
  private readonly http = inject(HttpClient);
  private readonly tokenStorage = inject(TokenStorageService);
  private readonly router = inject(Router);

  private readonly baseUrl = `${environment.apiUrl}/auth`;

  private readonly currentUserSignal = signal<CurrentUser | null>(this.readUserFromStoredToken());

  readonly currentUser = this.currentUserSignal.asReadonly();
  readonly isAuthenticated = computed(() => this.currentUserSignal() !== null);
  readonly isAdministrator = computed(() => this.currentUserSignal()?.role === 'Administrator');

  login(request: LoginRequest): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.baseUrl}/login`, request, { withCredentials: true }).pipe(
      tap((tokens) => {
        this.clearTwoFactorPromptState();
        this.applyTokens(tokens);
      }),
    );
  }

  hasSeenTwoFactorPrompt(): boolean {
    return typeof sessionStorage !== 'undefined'
      && sessionStorage.getItem(AuthService.twoFactorPromptStorageKey) === 'true';
  }

  markTwoFactorPromptSeen(): void {
    if (typeof sessionStorage !== 'undefined') {
      sessionStorage.setItem(AuthService.twoFactorPromptStorageKey, 'true');
    }
  }

  getTwoFactorStatus(): Observable<TwoFactorStatusResponse> {
    return this.http.get<TwoFactorStatusResponse>(`${this.baseUrl}/2fa/status`);
  }

  setupTwoFactor(): Observable<TwoFactorSetupResponse> {
    return this.http.post<TwoFactorSetupResponse>(`${this.baseUrl}/2fa/setup`, {});
  }

  confirmTwoFactor(code: string, rememberDevice: boolean): Observable<void> {
    return this.http.post<void>(
      `${this.baseUrl}/2fa/confirm`,
      { code, rememberDevice },
      { withCredentials: true },
    );
  }

  disableTwoFactor(code: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/2fa/disable`, { code }, { withCredentials: true });
  }

  refresh(): Observable<TokenResponse> {
    const body: RefreshTokenRequest = { refreshToken: this.tokenStorage.refreshToken ?? '' };
    return this.http.post<TokenResponse>(`${this.baseUrl}/refresh`, body).pipe(
      tap((tokens) => this.applyTokens(tokens)),
    );
  }

  logout(): void {
    const refreshToken = this.tokenStorage.refreshToken;
    this.clearTwoFactorPromptState();
    this.tokenStorage.clear();
    this.currentUserSignal.set(null);

    if (refreshToken) {
      const body: LogoutRequest = { refreshToken };
      this.http.post(`${this.baseUrl}/logout`, body).subscribe({ error: () => undefined });
    }

    this.router.navigateByUrl('/login');
  }

  get accessToken(): string | null {
    return this.tokenStorage.accessToken;
  }

  get refreshToken(): string | null {
    return this.tokenStorage.refreshToken;
  }

  private applyTokens(tokens: TokenResponse): void {
    this.tokenStorage.save(tokens);
    this.currentUserSignal.set(this.parseUser(tokens.accessToken));
  }

  private clearTwoFactorPromptState(): void {
    if (typeof sessionStorage !== 'undefined') {
      sessionStorage.removeItem(AuthService.twoFactorPromptStorageKey);
    }
  }

  private readUserFromStoredToken(): CurrentUser | null {
    const token = this.tokenStorage.accessToken;
    return token ? this.parseUser(token) : null;
  }

  private parseUser(token: string): CurrentUser | null {
    const payload = decodeJwtPayload(token);
    if (!payload) {
      return null;
    }

    const login = readClaim(payload, 'name') ?? readClaim(payload, 'sub');
    const role = readClaim(payload, 'role') ?? '';
    const permissions = readClaimArray(payload, 'permission');

    if (!login) {
      return null;
    }

    return { login, role, permissions };
  }
}
