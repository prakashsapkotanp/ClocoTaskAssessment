import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class MusicService {
  private base(artistId: number) {
    return `/api/artist/${artistId}/music`;
  }

  constructor(private http: HttpClient) {}

  getSongs(artistId: number) {
    return this.http.get<any[]>(this.base(artistId));
  }

  createSong(artistId: number, data: any) {
    return this.http.post<any>(this.base(artistId), data);
  }

  updateSong(artistId: number, id: number, data: any) {
    return this.http.put<any>(`${this.base(artistId)}/${id}`, data);
  }

  deleteSong(artistId: number, id: number) {
    return this.http.delete<any>(`${this.base(artistId)}/${id}`);
  }
}
