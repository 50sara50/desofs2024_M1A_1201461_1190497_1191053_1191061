using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using StreamingPlatform.Dao.Properties;

namespace StreamingPlatform.Dao.Helper
{
    public static class ModelPropertyEncrypterExtension
    {
        public static void UseEncryption(this ModelBuilder modelBuilder)
        {
            SecureDataEncryptorConvertor converter = new();

            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (IMutableProperty property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string) && !IsDiscriminator(property))
                    {
                        var attributes = property.PropertyInfo?.GetCustomAttributes(typeof(SecurePropertyAttribute), false);
                        if (attributes != null && attributes.Length != 0)
                        {
                            property.SetValueConverter(converter);
                        }
                    }
                }
            }
        }

        private static bool IsDiscriminator(IMutableProperty property)
        {
            return property.Name == "Discriminator" || property.PropertyInfo == null;
        }
    }
}
