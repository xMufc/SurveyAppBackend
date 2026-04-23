using System.ComponentModel.DataAnnotations;

namespace Backend.Models
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
