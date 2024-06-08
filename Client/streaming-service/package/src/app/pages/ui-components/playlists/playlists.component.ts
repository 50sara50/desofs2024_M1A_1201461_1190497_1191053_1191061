import { Component, OnInit } from '@angular/core';
import { Playlist } from '../domain/Playlist';
import { PlaylistService } from 'src/app/services/playlist.service';
import { SongService } from 'src/app/services/song.service';

export interface Section {
  name: string;
  updated: Date;
}

@Component({
  selector: 'app-lists',
  templateUrl: './playlists.component.html',
})
export class AppListsComponent implements OnInit{
  userEmail: 'user@example.com';
  playlist: Playlist;
  title: string;
  songs: string[];

  playlists: Playlist[];

  constructor(private playlistServcie: PlaylistService, private songService: SongService) {}
  ngOnInit(): void {
    this.getPlaylists();
  }

  public getPlaylists(): void{
    this.playlistServcie.getPlaylists(this.userEmail).subscribe((data) => {
      return this.playlists = data;
    });
  }

}
