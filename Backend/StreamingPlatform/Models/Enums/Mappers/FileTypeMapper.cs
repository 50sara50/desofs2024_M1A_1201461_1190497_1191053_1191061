namespace StreamingPlatform.Models.Enums.Mappers
{
    public static class FileTypeMapper
    {
        private static readonly Dictionary<string, FileType> FileTypeMapperValue = new()
        {
            { "audio/mpeg", FileType.MP3 },
            { "audio/x-m4a", FileType.M4A },
            { "audio/wav", FileType.WAV },
                { "text/plain", FileType.TXT } // Adding the .txt MIME type

        };

        public static FileType? ExtensionToFilePath(string fileExtension)
        {
            if (FileTypeMapperValue.TryGetValue(fileExtension, out FileType result))
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