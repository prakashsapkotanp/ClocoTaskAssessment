import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ArtistService {
  private readonly API = '/api/artist';

  constructor(private http: HttpClient) { }

  getArtists(page = 1, pageSize = 10) {
    return this.http.get<any>(`${this.API}?page=${page}&pageSize=${pageSize}`);
  }

  createArtist(data: any) {
    return this.http.post<any>(this.API, data);
  }

  updateArtist(id: number, data: any) {
    return this.http.put<any>(`${this.API}/${id}`, data);
  }

  deleteArtist(id: number) {
    return this.http.delete<any>(`${this.API}/${id}`);
  }

  exportCsv() {
    return this.http.get(`${this.API}/export`, { responseType: 'blob' });
  }

  importCsv(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<any>(`${this.API}/import`, formData);
  }

  getArtistByUserId(userId: number) {
    return this.http.get<any>(`${this.API}/me/${userId}`);
  }
}
