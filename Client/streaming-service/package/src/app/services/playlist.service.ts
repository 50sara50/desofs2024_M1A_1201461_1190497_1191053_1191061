import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { SongService } from './song.service';
import { PlaylistResponse } from '../model/response/PlaylistResponse';

@Injectable({
  providedIn: 'root',
})
export class PlaylistService {
  public Url = 'https://localhost:7255/Playlist';
  public AuthUrl = 'https://localhost:7255/Auth/';
  userIdData: string;
  playlists: any;

  constructor(
    private httpClient: HttpClient,
    private songService: SongService
  ) {}

  public extractData(res: any) {
    return res || {};
  }

  getPlaylists(): Observable<PlaylistResponse[]> {
    const url = `${this.Url}/GetUserPlaylists`;

    return this.getUserId().pipe(
      switchMap((userIdData) => {
        this.userIdData = userIdData.message;
        return this.httpClient.get(`${url}?userId=${this.userIdData}`);
      }),
      map(this.extractData)
    );
  }

  getUserId(): Observable<{
    message: string;
  }> {
    return this.httpClient.get(`${this.AuthUrl}user-id`).pipe(this.extractData);
  }

  loadPlaylistSongs(playlists: PlaylistResponse[]): void {
    playlists.forEach((playlist) => {
      playlist.songs?.forEach((song: string) => {
        return this.songService.getSong(song);
      });
    });
  }
}
