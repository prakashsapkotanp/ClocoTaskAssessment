import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MusicService } from '../../../core/services/music';
import { ArtistService } from '../../../core/services/artist';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-song-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './song-list.component.html',
  styleUrls: ['./song-list.component.css']
})
export class SongListComponent implements OnInit {
  songs: any[] = [];
  artistId!: number;
  loading = false;
  showModal = false;
  editingSong: any = null;
  form: FormGroup;
  toast = '';
  toastType: 'success' | 'error' = 'success';
  genres = ['Rnb', 'Country', 'Classic', 'Rock', 'Jazz'];

  selectedFile: File | null = null;

  constructor(
    private musicService: MusicService,
    private artistService: ArtistService,
    public authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      title: ['', Validators.required],
      albumName: [''],
      genre: ['', Validators.required]
    });
  }

  get role() { return this.authService.getRole(); }

  ngOnInit() {
    this.artistId = Number(this.route.snapshot.paramMap.get('artistId'));
    this.load();
  }

  load() {
    this.loading = true;
    this.musicService.getSongs(this.artistId).subscribe({
      next: (res) => { this.songs = res; this.loading = false; },
      error: () => this.loading = false
    });
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  openModal(s?: any) {
    this.editingSong = s;
    this.selectedFile = null;
    if (s) this.form.patchValue(s);
    else this.form.reset();
    this.showModal = true;
  }

  save() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    this.loading = true;
    const action = this.editingSong
      ? this.musicService.updateSong(this.artistId, this.editingSong.id, this.form.value)
      : this.musicService.createSong(this.artistId, this.form.value, this.selectedFile || undefined);

    action.subscribe({
      next: () => {
        this.showToast('Saved!', 'success');
        this.showModal = false;
        this.load();
      },
      error: (err: any) => {
        this.loading = false;
        this.showToast(err.error?.message || 'Error saving song.', 'error');
      }
    });
  }

  download(s: any) {
    this.musicService.downloadSong(this.artistId, s.id).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${s.title}.mp3`;
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  delete(s: any) {
    if (!confirm(`Delete song "${s.title}"?`)) return;
    this.musicService.deleteSong(this.artistId, s.id).subscribe({
      next: () => { this.showToast('Song deleted.', 'success'); this.load(); },
      error: () => this.showToast('Error deleting.', 'error')
    });
  }

  goBack() { this.router.navigate(['/dashboard/artists']); }

  showToast(msg: string, type: 'success' | 'error') {
    this.toast = msg; this.toastType = type;
    setTimeout(() => this.toast = '', 3000);
  }
}
