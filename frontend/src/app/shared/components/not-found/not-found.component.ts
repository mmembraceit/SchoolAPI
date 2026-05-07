import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div style="text-align:center; padding: 4rem;">
      <h1>404 — Page Not Found</h1>
      <p>The page you are looking for does not exist.</p>
      <a routerLink="/students">Go home</a>
    </div>
  `,
})
export class NotFoundComponent {}
