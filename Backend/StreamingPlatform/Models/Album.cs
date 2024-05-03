namespace StreamingPlatform;

public class Album
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public Guid ArtistId { get; set; }

    public Album(Guid id, string title, Guid artistId)
    {
        this.Id = id;
        this.Title = title;
        this.ArtistId = artistId;
    }

    public Album()
    {
        this.Id = Guid.NewGuid();
        this.Title = string.Empty;
        this.ArtistId = Guid.NewGuid();
    }
}