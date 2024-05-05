using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StreamingPlatform.Utils;

namespace StreamingPlatform.Dao.Helper
{

    public class SecureDataEncryptorConvertor(ConverterMappingHints? mappingHints = null) : ValueConverter<string, string>(x => SecureDataEncriptionHelper.Encrypt(x), x => SecureDataEncriptionHelper.Decrypt(x), mappingHints)
    {
    }
}
