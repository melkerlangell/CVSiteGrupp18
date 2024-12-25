using System.ComponentModel.DataAnnotations;

namespace CVSiteGrupp18.Models
{
    public class LogInViewModel
    {
        [Required(ErrorMessage ="Ange ditt användarnamn")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Ange ditt lösenord")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Kom ihåg mig")]
        public bool RememberMe { get; set; }
    }
}
