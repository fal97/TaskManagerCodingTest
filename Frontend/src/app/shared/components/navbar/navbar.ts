import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Router } from '@angular/router';

import { AuthenticationService } from '../../../features/authentication/services';

@Component({
  selector: 'app-navbar',
  imports: [MatButtonModule, MatToolbarModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  protected readonly authenticationService = inject(AuthenticationService);
  private readonly router = inject(Router);

  protected logout(): void {
    this.authenticationService.logout().subscribe({
      next: () => void this.router.navigate(['/login']),
    });
  }
}
