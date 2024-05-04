using System.ComponentModel.DataAnnotations;

namespace StreamingPlatform.Models
{
    /// <summary>
    /// The user's password.
    /// </summary>
    public class Password
    {
        //TODO: We need to define if we are going to have an Id for the password. We need it to make the relationship with the User.
        [Key]
        public Guid Id { get; set; }

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