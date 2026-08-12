import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { ActivatedRoute } from '@angular/router';

import { AuthenticationService } from '../../services';

@Component({
  selector: 'app-login',
  imports: [MatButtonModule, MatCardModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private readonly authenticationService = inject(AuthenticationService);
  private readonly route = inject(ActivatedRoute);

  protected login(): void {
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/tasks';
    void this.authenticationService.login({
      redirectUri: `${window.location.origin}${returnUrl}`,
    });
  }

  protected register(): void {
    void this.authenticationService.register(`${window.location.origin}/tasks`);
  }
}
