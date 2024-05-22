using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using StreamingPlatform.Models.Enums;

namespace StreamingPlatform.Dtos.Contract;

public class NewUserContract
{
    /// <summary>
    /// The user's password.
    /// </summary>
    private string _password;

    /// <summary>
    /// The user's username.
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(25, ErrorMessage = "Username is too long. Max length is 25 characters.")]
    public string UserName { get; set; }

    /// <summary>
    /// The user's email.
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    /// <summary>
    /// The user's password.
    /// The password must contain at least one uppercase letter, one lowercase letter,
    /// one number, one special character and 12 characters long minimum.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]

    // ASVS#2.1.1
    [MinLength(12, ErrorMessage = "Password is too short. Min length is 12 characters.")]

    // ASVS#2.1.2 
    
    [MaxLength(128, ErrorMessage = "Password is too long. Max length is 128 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character.")]
    public string Password
    { 
        get {return _password;}

        //ASVS#2.1.3  Replace multiple spaces with a single space
        set{_password = Regex.Replace(value, @"\s+", " ");} 
    }

    /// <summary>
    /// The user's name.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50, ErrorMessage = "Name is too long. Max length is 50 characters.")]
    public string Name { get; set; }

    /// <summary>
    /// The user's age.
    /// </summary>
    [Required(ErrorMessage = "Age is required")]
    [Range(10, 99, ErrorMessage = "Age must be between 10 and 99")]
    public int Age { get; set; }

    /// <summary>
    /// The user's address.
    /// </summary>
    public string Address { get; set; }
    
    [Required(ErrorMessage = "Role is required")]
    [EnumDataType(typeof(Role), ErrorMessage = "Invalid role")]
    public Role Role { get; set; }
}
