using System.ComponentModel.DataAnnotations;

namespace SurveyAppBackend.DTOs.User
{
    public class Register
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]

        public string Password { get; set; }

    }
}
