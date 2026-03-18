import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  form: FormGroup;
  loading = false;
  error = '';
  success = '';

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.form = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      phone: [''],
      dob: ['', Validators.required],
      gender: ['', Validators.required],
      address: [''],
      // role: ['super_admin', Validators.required]
    });
  }

  onSubmit() {
    this.form.value.role = 'super_admin'; // hardcoding since in register default role is super_admin
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.error = '';
    this.authService.register(this.form.value).subscribe({
      next: () => {
        this.success = 'Registered successfully! Redirecting to login...';
        setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      error: (err) => {
        this.error = err.error?.message || 'Registration failed.';
        this.loading = false;
      }
    });
  }
}
