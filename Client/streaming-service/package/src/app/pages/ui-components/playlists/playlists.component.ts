import { Component, OnInit } from '@angular/core';
import { Playlist } from '../domain/Playlist';
import { PlaylistService } from 'src/app/services/playlist.service';

export interface Section {
  name: string;
  updated: Date;
}

@Component({
  selector: 'app-lists',
  templateUrl: './playlists.component.html',
})
export class AppListsComponent implements OnInit{
  playlist: Playlist;
  title: string;
  songs: string[];

  playlists: Playlist[];

  constructor(private playlistServcie: PlaylistService) {}
  ngOnInit(): void {
    this.getPlaylists();
  }

  public getPlaylists(): void{
    this.playlistServcie.getPlaylists().subscribe((data) => {
      this.playlists = data;
    });
    console.log(this.playlistServcie.getPlaylists());
  }

}
