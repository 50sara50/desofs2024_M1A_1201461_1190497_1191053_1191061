import { HttpBackend, HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { MessageService } from "./message.service";
import { Observable, map } from "rxjs";

@Injectable({
    providedIn: 'root',
})

export class PlaylistService {
    public Url = 'https://localhost:32774/Playlist'

    constructor(
        private httpClient: HttpClient,
        private messageService: MessageService
    ){}

    public extractData(res: any){
        return res || {};
    }

    getPlaylists(params?: string): Observable<any>{
        var url = `${this.Url}/GetUserPlaylists`;
        if(params){
            return this.httpClient
                .get(`${url}?${params}`)
                .pipe(map(this.extractData));
        } else {
            return this.httpClient.get(url).pipe(map(this.extractData));
        }
    }
}