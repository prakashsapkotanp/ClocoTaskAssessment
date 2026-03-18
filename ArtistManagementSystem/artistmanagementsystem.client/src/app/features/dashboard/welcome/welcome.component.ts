import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="welcome-container">
      <h1>Welcome, {{ userName }}!</h1>      
      <div class="info-card glass-panel" *ngIf="role === 'artist'">
        <h3>Artist Access</h3>
        <p>As an artist, you can manage your songs. Please contact your manager if you don't see your profile link.</p>
      </div>
      
      <div class="info-card glass-panel" *ngIf="role !== 'artist'">
        <h3>System Overview</h3>
        <p>Use the sidebar to manage users and artists.</p>
      </div>
    </div>
  `,
  styles: [`
    .welcome-container { padding: 2rem; }
    .info-card { padding: 1.5rem; margin-top: 1rem; border-radius: 12px; max-width: 500px; }
    .badge-role { font-weight: bold; color: var(--accent-color, #7000ff); }
  `]
})
export class WelcomeComponent {
  constructor(private authService: AuthService) { }
  get userName() { return this.authService.getUserName(); }
  get role() { return this.authService.getRole(); }
}
