using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EventManagerWeb.Models
{
    public class User: IdentityUser
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "First Name should be atleast 3 characters!")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Last Name should be atleast 3 characters!")]
        public string LastName { get; set; }
        [EmailAddress]

        //[Required]
        //[DataType(DataType.Password)]
        //[RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contain atleast 1 uppercase, 1 lowercase and 1 number!")]
        //[StringLength(255, MinimumLength = 8, ErrorMessage = "Password should be atleast 8 characters long!")]
        ////public override string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //[NotMapped]
        //[Compare("PasswordHash")]
        //[DataType(DataType.Password)]
        //public string Confirm { get; set; }

        public List<Association> UserEvents { get; set; }
    }
}

