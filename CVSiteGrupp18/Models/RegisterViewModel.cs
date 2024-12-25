using System.ComponentModel.DataAnnotations;

namespace CVSiteGrupp18.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ange ett användarnamn")]
        [MaxLength(30, ErrorMessage = "Användarnamnet får inte vara längre än 30 tecken")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Ange en e-mail")]
        [EmailAddress(ErrorMessage ="Ange din e-mail i giltigt format")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Ange ett lösenord")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="Bekräfta lösenord")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Lösenordet matchar inte det du angav")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Ange din adress"), MaxLength(100)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Ange om din profil är publik")]
        public bool isPublic { get; set; }
    }
}
