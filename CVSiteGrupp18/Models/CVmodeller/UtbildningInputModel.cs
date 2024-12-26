using System.ComponentModel.DataAnnotations;

namespace CVSiteGrupp18.Models.CVmodeller
{
    public class UtbildningInputModel
    {
        [Required(ErrorMessage = "Lärosäte är obligatoriskt.")]
        [MaxLength(200)]
        public string Skola { get; set; }

        [MaxLength(200)]
        public string Titel { get; set; }

        [Required]
        public DateTime Startdatum { get; set; }

        public DateTime? Slutdatum { get; set; }
    }
}
