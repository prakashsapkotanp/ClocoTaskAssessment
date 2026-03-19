import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth';
import { ArtistService } from '../../../core/services/artist';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.css']
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