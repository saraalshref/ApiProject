using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Api_Project.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
        public int Age { get; set; }

      

    }
}
