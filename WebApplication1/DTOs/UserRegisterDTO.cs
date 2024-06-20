using System.ComponentModel.DataAnnotations;

namespace Api_Project.DTOS
{
    public class UserRegisterDTO
    {
        [Required]
        public string Name { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120.")]
        public int Age { get; set; }

       
    }
}
