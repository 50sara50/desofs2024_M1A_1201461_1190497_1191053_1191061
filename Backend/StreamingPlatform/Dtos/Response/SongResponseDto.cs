namespace StreamingPlatform.Dtos.Response
{
    public class SongResponseDto (string title, string artist, string duration, string? album)
    {
        public string Title { get; set; } = title;

        public string Artist { get; set; } = artist;

        public string Duration { get; set; } = duration;

        public string? Album { get; set; } = album;
    }
}