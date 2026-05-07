import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div style="text-align:center; padding: 4rem;">
      <h1>403 — Unauthorized</h1>
      <p>You do not have permission to access this page.</p>
      <a routerLink="/students">Go back</a>
    </div>
  `,
})
export class UnauthorizedComponent {}
