using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace StreamingPlatform.Dtos.Contract
{
    public class ChangePasswordContract
    {
        /// <summary>
        /// The user's new password.
        /// </summary>
        private string _newPassword;
        
        /// <summary>
        /// The user's new password.
        /// </summary>
        [Required(ErrorMessage = "Old Password is required")]
        [MaxLength(128, ErrorMessage = "Password is too long. Max length is 128 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character.")]
        public string NewPassword
        { 
            get {return _newPassword;}

            //ASVS#2.1.3  Replace multiple spaces with a single space
            set{_newPassword = Regex.Replace(value, @"\s+", " ");} 
        }

        /// <summary>
        /// Old password.
        /// </summary>
        [Required(ErrorMessage = "Old Password is required")]
        [MaxLength(128, ErrorMessage = "Password is too long. Max length is 128 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character.")]
        public string OldPassword { get; set; }
        
        /// <summary>
        /// The user's email.
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}