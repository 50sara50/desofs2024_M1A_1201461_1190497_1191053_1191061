namespace StreamingPlatform.Dtos.Response
{
    public class SongResponseDto(string title, string? artist, string? album, string? downloadUrl = null)
    {
        public string Title { get; set; } = title;

        public string? Artist { get; set; } = artist;

        public string? Album { get; set; } = album;

        public string? DownloadUrl { get; set; } = downloadUrl;
    }
}