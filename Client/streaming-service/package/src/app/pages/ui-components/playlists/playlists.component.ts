import { Component, OnInit } from '@angular/core';
import { Playlist } from '../domain/Playlist';
import { PlaylistService } from 'src/app/services/playlist.service';
import { SongService } from 'src/app/services/song.service';
import { Song } from '../domain/Song';

export interface Section {
  name: string;
  updated: Date;
}

@Component({
  selector: 'app-lists',
  templateUrl: './playlists.component.html',
})
export class AppListsComponent implements OnInit {
  userEmail: 'user@example.com';
  playlist: Playlist;
  title: string;

  playlists: Playlist[];

  constructor(
    private playListService: PlaylistService,
    private songService: SongService
  ) {}
  ngOnInit(): void {
    this.getPlaylists();
  }

  public getPlaylists(): void {
    this.playListService.getPlaylists().subscribe((data) => {
      this.playlists = data;
      this.playlists.forEach((playlist) => {
        playlist.songsInfo = [];
        playlist.songs?.forEach((song) => {
          this.songService.getSong(song).subscribe((data) => {
            console.log('Data', data);
            playlist.songsInfo?.push(data);
          });
        });
      });
      return data;
    });
  }
}
