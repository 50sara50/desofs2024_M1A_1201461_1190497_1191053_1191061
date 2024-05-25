namespace StreamingPlatform.Dtos.Response
{
    public class DownloadSongResponse(string name, string fileType, byte[] data)
    {

        public string Name { get; set; } = name;

        public string FileType { get; set; } = fileType;

        public byte[] Data { get; set; } = data;
    }
}
