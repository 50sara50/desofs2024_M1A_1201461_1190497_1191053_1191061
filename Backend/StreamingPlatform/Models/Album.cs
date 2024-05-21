using StreamingPlatform.Models;

namespace StreamingPlatform;

public class Album
{
    public Album(Guid id, string title, User artistId)
    {
        this.Id = id;
        this.Title = title;
        this.Artist = artistId;
        this.SongList = [];
    }

    public Album()
    {
        this.Id = Guid.NewGuid();
        this.Title = string.Empty;
        this.Artist = null;
        this.SongList = [];
    }

    public Guid Id { get; set; }

    public string Title { get; set; }

    public User? Artist { get; set; }

    public ICollection<Song> SongList { get; set; }

    public void AddSong(Song song)
    {
        if (this.SongList.Contains(song))
        {
            throw new Exception($"This song has already been added to the album '${this.Title}'");
        }

        this.SongList.Add(song);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }

        Album album = (Album)obj;
        return this.Id == album.Id
               && this.Title == album.Title
               && Guid.Equals(this.Artist, album.Artist);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Id, this.Title, this.Artist);
    }
}