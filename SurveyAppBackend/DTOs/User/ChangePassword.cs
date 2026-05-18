using System.ComponentModel.DataAnnotations;

namespace SurveyAppBackend.DTOs.User
{
    public class ChangePassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

    }
}
