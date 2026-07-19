import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { catchError, Observable, of, tap } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { AuthenticatedUser, LoginRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private readonly http = inject(HttpClient);
  private readonly resourceUrl = `${environment.apiBaseUrl}/api/auth`;
  private readonly userState = signal<AuthenticatedUser | null>(null);

  readonly currentUser = this.userState.asReadonly();

  login(request: LoginRequest): Observable<AuthenticatedUser> {
    return this.http
      .post<AuthenticatedUser>(`${this.resourceUrl}/login`, request)
      .pipe(tap((user) => this.userState.set(user)));
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
      .pipe(tap(() => this.userState.set(null)));
  }

  clearSession(): void {
    this.userState.set(null);
  }
}
