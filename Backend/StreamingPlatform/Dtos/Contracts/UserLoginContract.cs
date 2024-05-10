using System.ComponentModel.DataAnnotations;

namespace StreamingPlatform.Dtos.Contracts;
public class UserLoginContract
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}
