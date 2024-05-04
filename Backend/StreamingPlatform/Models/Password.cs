namespace StreamingPlatform.Models
{
    /// <summary>
    /// The user's password.
    /// </summary>
    public class Password
    {
        /// <summary>
        /// The password's value.
        /// </summary>
        public string Value { get; set; }
        
        /// <summary>
        /// The password's hash.
        /// </summary>
        public string Salt { get; set; }

        public Password(string value)
        {
            this.Value = value;
            //TODO salt
        }
    }
}