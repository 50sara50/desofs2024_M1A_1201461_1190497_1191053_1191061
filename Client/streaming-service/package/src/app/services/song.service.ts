import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { MessageService } from "./message.service";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root', 
})
export class SongService{
    public Url = 'https://localhost:7255/api/song';

    constructor(
        private httpClient: HttpClient,
        private messageService: MessageService
    ){}

    public extractData(res: any){
        return res || {};
    }

    getSong(songId: string): Observable<any>{
        return this.httpClient.get(this.Url).pipe(this.extractData);
    }
}