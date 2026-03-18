import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ArtistService } from '../../../core/services/artist';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-artist-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './artist-list.component.html',
  styleUrls: ['./artist-list.component.css']
})
export class ArtistListComponent implements OnInit {
  artists: any[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 10;
  loading = false;
  showModal = false;
  editingArtist: any = null;
  form: FormGroup;
  toast = '';
  toastType: 'success' | 'error' = 'success';

  constructor(
    private artistService: ArtistService,
    public authService: AuthService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      dob: ['', Validators.required],
      gender: ['', Validators.required],
      address: [''],
      firstReleaseYear: ['', [Validators.required, Validators.min(1900)]],
      noOfAlbumsReleased: [0, [Validators.required, Validators.min(0)]]
    });
  }

  get role() { return this.authService.getRole(); }

  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    this.artistService.getArtists(this.page, this.pageSize).subscribe({
      next: (res) => { this.artists = res.artists || res.data || []; this.totalCount = res.totalCount; this.loading = false; },
      error: () => this.loading = false
    });
  }

  openModal(a?: any) {
    this.editingArtist = a;
    if (a) {
      this.form.patchValue({ ...a, dob: a.dob?.split('T')[0] });
      this.form.get('email')?.clearValidators();
      this.form.get('password')?.clearValidators();
    } else {
      this.form.reset();
      this.form.get('email')?.setValidators([Validators.required, Validators.email]);
      this.form.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    }
    this.form.get('email')?.updateValueAndValidity();
    this.form.get('password')?.updateValueAndValidity();
    this.showModal = true;
  }

  save() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const action = this.editingArtist
      ? this.artistService.updateArtist(this.editingArtist.id, this.form.value)
      : this.artistService.createArtist(this.form.value);
    action.subscribe({
      next: () => { this.showToast('Saved!', 'success'); this.showModal = false; this.load(); },
      error: (err) => this.showToast(err.error?.message || 'Error.', 'error')
    });
  }

  delete(a: any) {
    if (!confirm(`Delete artist ${a.name}?`)) return;
    this.artistService.deleteArtist(a.id).subscribe({
      next: () => { this.showToast('Artist deleted.', 'success'); this.load(); },
      error: () => this.showToast('Error deleting artist.', 'error')
    });
  }

  viewSongs(a: any) { this.router.navigate(['/dashboard/artists', a.id, 'songs']); }

  exportCsv() {
    this.artistService.exportCsv().subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url; a.download = 'artists.csv'; a.click();
      URL.revokeObjectURL(url);
    });
  }

  importCsv(event: any) {
    const file: File = event.target.files[0];
    if (!file) return;
    this.artistService.importCsv(file).subscribe({
      next: (res) => { this.showToast(res.message, 'success'); this.load(); },
      error: () => this.showToast('Import failed.', 'error')
    });
    event.target.value = '';
  }

  get totalPages() { return Math.ceil(this.totalCount / this.pageSize); }
  prevPage() { if (this.page > 1) { this.page--; this.load(); } }
  nextPage() { if (this.page < this.totalPages) { this.page++; this.load(); } }


  showToast(msg: string, type: 'success' | 'error') {
    this.toast = msg; this.toastType = type;
    setTimeout(() => this.toast = '', 3000);
  }
}
