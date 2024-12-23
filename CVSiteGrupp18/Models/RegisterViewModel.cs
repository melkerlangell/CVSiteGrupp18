using System.ComponentModel.DataAnnotations;

namespace CVSiteGrupp18.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Lösenordet matchar inte det du angav")]
        public string ConfirmPassword { get; set; }
    }
}
