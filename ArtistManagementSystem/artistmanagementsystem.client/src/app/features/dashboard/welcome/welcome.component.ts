import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth';
import { ArtistService } from '../../../core/services/artist';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="welcome-container">
      <h1 class="welcome-title">Welcome, {{ userName }}!</h1>      
      
      <div class="info-card glass-panel artist-card" *ngIf="role === 'artist'">
        <div class="card-header">
          <span class="card-icon">🎵</span>
          <h3>Artist Dashboard</h3>
        </div>
        <p class="card-desc">Welcome to your personal music space. Here you can upload, edit, and organize your songs.</p>
        
        <div *ngIf="artistId" class="card-actions">
          <a [routerLink]="['/dashboard/artists', artistId, 'songs']" class="btn btn-primary">
            Manage My Songs
          </a>
        </div>

        <div *ngIf="!artistId" class="notice-box">
          <p><strong>Profile Pending:</strong> Your artist profile hasn't been linked by a manager yet. Please contact your administrator to set up your profile so you can start managing songs.</p>
        </div>
      </div>
      
      <div class="info-card glass-panel" *ngIf="role !== 'artist'">
        <div class="card-header">
          <span class="card-icon">⚙️</span>
          <h3>System Management</h3>
        </div>
        <p>Use the sidebar navigation to manage user accounts, artist profiles, and the global music database.</p>
      </div>
    </div>
  `,
  styles: [`
    .welcome-container { padding: 3rem; max-width: 1000px; }
    .welcome-title { margin-bottom: 2rem; font-size: 2.5rem; font-weight: 800; }
    .info-card { padding: 2rem; border-radius: var(--radius-lg, 16px); background: rgba(255, 255, 255, 0.05); }
    .card-header { display: flex; align-items: center; gap: 1rem; margin-bottom: 1rem; }
    .card-icon { font-size: 1.8rem; }
    .card-desc { color: var(--text-muted); margin-bottom: 1.5rem; line-height: 1.6; }
    .notice-box { padding: 1rem; background: rgba(239, 68, 68, 0.1); border-left: 4px solid #ef4444; border-radius: 4px; font-size: 0.9rem; }
    .card-actions { margin-top: 1rem; }
  `]
})
export class WelcomeComponent implements OnInit {
  artistId: number | null = null;

  constructor(
    private authService: AuthService,
    private artistService: ArtistService
  ) { }

  ngOnInit() {
    if (this.role === 'artist') {
      const userId = Number(this.authService.getUserId());
      if (userId) {
        this.artistService.getArtistByUserId(userId).subscribe({
          next: (a) => this.artistId = a?.id || null,
          error: () => this.artistId = null
        });
      }
    }
  }

  get userName() { return this.authService.getUserName(); }
  get role() { return this.authService.getRole(); }
}
