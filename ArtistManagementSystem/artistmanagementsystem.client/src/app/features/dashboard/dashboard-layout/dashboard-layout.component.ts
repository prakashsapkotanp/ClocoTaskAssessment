import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth';
import { ArtistService } from '../../../core/services/artist';

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard-layout.component.html',
  styleUrls: ['./dashboard-layout.component.css']
})
export class DashboardLayoutComponent implements OnInit {
  artistId: number | null = null;

  constructor(
    public authService: AuthService,
    private artistService: ArtistService
  ) {}

  ngOnInit() {
    if (this.role === 'artist') {
      const userId = Number(this.authService.getUserId());
      if (userId) {
        this.artistService.getArtistByUserId(userId).subscribe({
          next: (artist) => this.artistId = artist?.id || null,
          error: () => this.artistId = null
        });
      }
    }
  }

  get role() { return this.authService.getRole(); }
  get userName() { return this.authService.getUserName(); }

  logout() { this.authService.logout(); }
}
