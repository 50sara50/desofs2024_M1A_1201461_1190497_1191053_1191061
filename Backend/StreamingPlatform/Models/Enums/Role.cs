namespace StreamingPlatform.Models.Enums
{
    /// <summary>
    /// Enum for User's Roles.
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Represents the role of the system's Administrator.
        /// </summary>
        Admin,
        
        /// <summary>
        /// Represents an user that has an active subscription
        /// </summary>
        Subscriber,
        
        /// <summary>
        /// Represents an user that is an artist
        /// </summary>
        Artist,
    }
}