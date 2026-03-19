import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../../core/services/user';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {
  users: any[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 10;
  loading = false;
  showModal = false;
  editingUser: any = null;
  form: FormGroup;
  toast = '';
  toastType: 'success' | 'error' = 'success';

  constructor(private userService: UserService, private authService: AuthService, private fb: FormBuilder) {
    this.form = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      phone: ['', Validators.pattern('^[0-9]{10}$')],
      dob: ['', Validators.required],
      gender: ['', Validators.required],
      address: [''],
      role: ['artist_manager', Validators.required]
    });
  }

  ngOnInit() { this.load(); }

  load() {
    this.loading = true;
    this.userService.getUsers(this.page, this.pageSize).subscribe({
      next: (res) => {
        this.users = res.data;
        this.totalCount = res.totalCount;
        this.loading = false;
        console.log(this.users);
      },
      error: () => { this.loading = false; }
    });
  }

  openModal(u?: any) {
    this.editingUser = u;
    if (u) {
      this.form.patchValue({ ...u, dob: u.dob?.split('T')[0], password: '' });
      this.form.get('password')?.clearValidators();
    } else {
      this.form.reset({ role: 'artist_manager' });
      this.form.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    }
    this.form.get('password')?.updateValueAndValidity();
    this.showModal = true;
  }

  save() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const data = { ...this.form.value };
    const action = this.editingUser
      ? this.userService.updateUser(this.editingUser.id, data)
      : this.userService.createUser(data);

    action.subscribe({
      next: () => { this.showToast('Saved successfully!', 'success'); this.showModal = false; this.load(); },
      error: (err) => this.showToast(err.error?.message || 'Error saving user.', 'error')
    });
  }

  delete(user: any) {
    if (!confirm(`Delete user ${user.firstName} ${user.lastName}?`)) return;
    this.userService.deleteUser(user.id).subscribe({
      next: () => { this.showToast('User deleted.', 'success'); this.load(); },
      error: () => this.showToast('Error deleting user.', 'error')
    });
  }

  get totalPages() { return Math.ceil(this.totalCount / this.pageSize); }
  prevPage() { if (this.page > 1) { this.page--; this.load(); } }
  nextPage() { if (this.page < this.totalPages) { this.page++; this.load(); } }

  showToast(msg: string, type: 'success' | 'error') {
    this.toast = msg; this.toastType = type;
    setTimeout(() => this.toast = '', 3000);
  }
}
