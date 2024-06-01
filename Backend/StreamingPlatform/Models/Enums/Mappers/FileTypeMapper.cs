namespace StreamingPlatform.Models.Enums.Mappers
{
    public static class FileTypeMapper
    {
        private static readonly Dictionary<string, FileType> FileTypeMapperValue = new()
        {
            { ".mp3", FileType.MP3 },
            { ".m4a", FileType.M4A },
            { ".wav", FileType.WAV },
            { ".txt", FileType.TXT },
        };

        public static FileType? ExtensionToFilePath(string fileExtension)
        {
            if (!FileTypeMapperValue.TryGetValue(fileExtension, out FileType result))
            {
                return null;
            }

            return result;
        }

        public static string? FileTypeToExtension(FileType fileType)
        {
            KeyValuePair<string, FileType> currentExtension = FileTypeMapperValue.FirstOrDefault(x => x.Value == fileType);
            if (currentExtension.Value != fileType)
            {
                return null;
            }

            return currentExtension.Key;
        }
    }
}