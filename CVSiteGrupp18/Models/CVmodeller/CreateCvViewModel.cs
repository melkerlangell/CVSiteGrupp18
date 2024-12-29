using System.ComponentModel.DataAnnotations;
using CVSiteGrupp18.Models.CVmodeller;
using Microsoft.IdentityModel.Tokens;

namespace CVSiteGrupp18.Models.CV.CV
{
    public class CreateCvViewModel
    {
        [Required(ErrorMessage = "Titel är obligatoriskt.")]
        [MaxLength(200, ErrorMessage = "Titeln får inte vara längre än 200 tecken.")]
        public string Titel { get; set; }

        [Required(ErrorMessage = "Minst en kompetens måste anges.")]
        public List<string> Kompetenser { get; set; } = new List<string>();


        [Required(ErrorMessage = "Utbildningar är obligatoriska.")]
        public List<UtbildningInputModel> Utbildningar { get; set; } = new List<UtbildningInputModel>();

        [Required(ErrorMessage = "Erfarenheter är obligatoriska.")]
        public List<ErfarenhetInputModel> Erfarenheter { get; set; } = new List<ErfarenhetInputModel>();

   
    }
}
