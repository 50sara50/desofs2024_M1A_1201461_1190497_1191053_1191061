import { HttpBackend, HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { MessageService } from "./message.service";
import { Observable, map } from "rxjs";
import { switchMap } from 'rxjs/operators';
import { AuthService } from "./auth.service";
import { Playlist } from "../pages/ui-components/domain/Playlist";
import { SongService } from "./song.service";

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
        private messageService: MessageService,
        private songService: SongService
    ){}

    public extractData(res: any){
        return res || {};
    }

    getPlaylists(email: string): Observable<any> {
        const url = `${this.Url}/GetUserPlaylists`;
    
        return this.getUserId('user@example.com').pipe(
            switchMap(userIdData => {
                this.userIdData = userIdData.message; // Store userIdData if needed
                return this.httpClient.get(`${url}?userId=${this.userIdData}`);
            }),
            map(this.extractData)
        );
    }

    getUserId(email: string): Observable<any> {
        return this.httpClient.get(`${this.AuthUrl}user-id?email=${email}`).pipe(this.extractData);
    }

    loadPlaylistSongs(playlists: any): any{
        playlists.forEach((playlist: { songs: string[]; }) => {
            playlist.songs?.forEach((song: string) => {
                return this.songService.getSong(song);
            })
        })
    }
}