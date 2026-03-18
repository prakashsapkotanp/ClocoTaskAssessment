import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly API = '/api/user';

  constructor(private http: HttpClient) {}

  getUsers(page = 1, pageSize = 10) {
    return this.http.get<any>(`${this.API}?page=${page}&pageSize=${pageSize}`);
  }

  createUser(data: any) {
    return this.http.post<any>(this.API, data);
  }

  updateUser(id: number, data: any) {
    return this.http.put<any>(`${this.API}/${id}`, data);
  }

  deleteUser(id: number) {
    return this.http.delete<any>(`${this.API}/${id}`);
  }
}
