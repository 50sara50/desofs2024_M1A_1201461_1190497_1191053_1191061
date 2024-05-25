namespace StreamingPlatform.Dtos.Response
{
    public class SongResponse(string title, string? artistName, string? albumName)
    {
        public string Title { get; set; } = title;

        public string? ArtistName { get; set; } = artistName;

        public string? AlbumName { get; set; } = albumName;

    }
}
