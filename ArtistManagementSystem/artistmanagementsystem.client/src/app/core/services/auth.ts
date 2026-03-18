import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'?: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly API = '/api/auth';
  private readonly TOKEN_KEY = 'auth_token';

  constructor(private http: HttpClient, private router: Router) {}

  login(email: string, password: string) {
    return this.http.post<any>(`${this.API}/login`, { email, password });
  }

  register(data: any) {
    return this.http.post<any>(`${this.API}/register`, data);
  }

  saveToken(token: string) {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private getClaim(key: keyof JwtPayload): string {
    const token = this.getToken();
    if (!token) return '';
    try {
      const decoded = jwtDecode<JwtPayload>(token);
      return decoded[key] || '';
    } catch {
      return '';
    }
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    try {
      const decoded = jwtDecode<any>(token);
      return decoded.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  getRole(): string {
    return this.getClaim('http://schemas.microsoft.com/ws/2008/06/identity/claims/role');
  }

  getUserId(): string {
    return this.getClaim('http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier');
  }

  getUserName(): string {
    return this.getClaim('http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name');
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    this.router.navigate(['/login']);
  }
}
