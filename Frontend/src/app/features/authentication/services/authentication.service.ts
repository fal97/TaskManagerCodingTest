import { Injectable, signal } from '@angular/core';
import Keycloak, { KeycloakLoginOptions, KeycloakTokenParsed } from 'keycloak-js';

import { environment } from '../../../../environments/environment';
import { AuthenticatedUser } from '../models';

interface TaskManagerToken extends KeycloakTokenParsed {
  preferred_username?: string;
}

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private readonly keycloak = new Keycloak(environment.keycloak);
  private readonly userState = signal<AuthenticatedUser | null>(null);

  readonly currentUser = this.userState.asReadonly();

  async initialize(): Promise<void> {
    const authenticated = await this.keycloak.init({
      onLoad: 'check-sso',
      pkceMethod: 'S256',
      checkLoginIframe: false,
    });

    this.updateUserState(authenticated);
    this.keycloak.onAuthSuccess = () => this.updateUserState(true);
    this.keycloak.onAuthLogout = () => this.updateUserState(false);
    this.keycloak.onTokenExpired = () => void this.refreshToken();
  }

  isAuthenticated(): boolean {
    return this.keycloak.authenticated === true;
  }

  login(options?: KeycloakLoginOptions): Promise<void> {
    return this.keycloak.login(options);
  }

  register(redirectUri: string): Promise<void> {
    return this.keycloak.register({ redirectUri });
  }

  logout(): Promise<void> {
    return this.keycloak.logout({ redirectUri: window.location.origin });
  }

  async getValidAccessToken(): Promise<string | null> {
    if (!this.isAuthenticated()) {
      return null;
    }

    await this.refreshToken();
    return this.keycloak.token ?? null;
  }

  private async refreshToken(): Promise<void> {
    try {
      await this.keycloak.updateToken(30);
    } catch {
      this.updateUserState(false);
      await this.login({ redirectUri: window.location.href });
    }
  }

  private updateUserState(authenticated: boolean): void {
    if (!authenticated) {
      this.userState.set(null);
      return;
    }

    const token = this.keycloak.tokenParsed as TaskManagerToken | undefined;
    this.userState.set({
      username: token?.preferred_username ?? token?.sub ?? 'User',
    });
  }
}
