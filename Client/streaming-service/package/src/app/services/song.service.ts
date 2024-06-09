import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SongResponse } from '../model/response/SongResponse';

@Injectable({
  providedIn: 'root',
})
export class SongService {
  public Url = 'https://localhost:7255/api/song';

  constructor(private httpClient: HttpClient) {}

  public extractData(res: any) {
    console.log('extractData', res);
    return res || {};
  }

  getSong(songId: string): Observable<SongResponse> {
    const requestUrl = new URL(this.Url + '/GetSongById');
    requestUrl.searchParams.append('id', songId);
    console.log(requestUrl.toString());

    return this.httpClient.get(requestUrl.toString()).pipe(this.extractData);
  }

  downloadSong(url: string) {
    return this.httpClient.get<Blob>(url, { responseType: 'blob' as 'json' });
  }
}
