using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StreamingPlatform.Models.Enums;

namespace StreamingPlatform.Models
{
    /// <summary>
    /// Represents a User.
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// The user's unique identifier.
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// The username displayed on the system.
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// The user's email.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// The user's password.
        /// </summary>
        public Password Password { get; set; }
        
        /// <summary>
        /// The user's name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The user's age.
        /// </summary>
        public int Age { get; set; }
        
        /// <summary>
        /// The user's address.
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// The user's role.
        /// </summary>
        public Role Role { get; set; }
        
        /// <summary>
        /// The user's registration date.
        /// </summary>
        public DateTime CreateOn { get; set; }

        public User()
        {
            this.UserName = string.Empty;
            this.Email = string.Empty;
            this.Password = new Password(string.Empty);
            this.Name = string.Empty;
            this.Age = 0;
            this.Address = string.Empty;
            this.CreateOn = DateTime.Now;
        }

        public User(Guid id, string username, string email, string password, string name, int age, string address, Role role)
        {
            this.Id = id;
            this.UserName = username;
            this.Email = email;
            this.Password = new Password(password);
            this.Name = name;
            this.Age = age;
            this.Address = address;
            this.Role = role;
            this.CreateOn = DateTime.Now;
        }
    }
}