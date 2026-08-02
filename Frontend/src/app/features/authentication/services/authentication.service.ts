import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, Observable, of, tap } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { AccessTokenResponse, AuthenticatedUser, LoginRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private static readonly ACCESS_TOKEN_KEY = 'task_manager_access_token';
  private readonly http = inject(HttpClient);
  private readonly resourceUrl = `${environment.apiBaseUrl}/api/auth`;
  private readonly userState = signal<AuthenticatedUser | null>(null);

  readonly currentUser = this.userState.asReadonly();

  login(request: LoginRequest): Observable<AccessTokenResponse> {
    return this.http
      .post<AccessTokenResponse>(`${this.resourceUrl}/login`, request)
      .pipe(
        tap((response) => {
          sessionStorage.setItem(AuthenticationService.ACCESS_TOKEN_KEY, response.accessToken);
          this.userState.set({ username: response.username });
        }),
      );
  }

  loadCurrentUser(): Observable<AuthenticatedUser | null> {
    return this.http.get<AuthenticatedUser>(`${this.resourceUrl}/me`).pipe(
      tap((user) => this.userState.set(user)),
      catchError(() => {
        this.userState.set(null);
        return of(null);
      }),
    );
  }

  logout(): Observable<void> {
    return this.http
      .post<void>(`${this.resourceUrl}/logout`, {})
      .pipe(tap(() => this.clearSession()));
  }

  clearSession(): void {
    sessionStorage.removeItem(AuthenticationService.ACCESS_TOKEN_KEY);
    this.userState.set(null);
  }

  getAccessToken(): string | null {
    return sessionStorage.getItem(AuthenticationService.ACCESS_TOKEN_KEY);
  }
}
