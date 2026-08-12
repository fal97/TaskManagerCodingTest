import { Component, inject, OnInit } from '@angular/core';

import { AuthenticationService } from '../../services';

@Component({
  selector: 'app-register',
  template: '<p>Redirecting to secure registration…</p>',
})
export class Register implements OnInit {
  private readonly authenticationService = inject(AuthenticationService);

  ngOnInit(): void {
    void this.authenticationService.register(`${window.location.origin}/tasks`);
  }
}
