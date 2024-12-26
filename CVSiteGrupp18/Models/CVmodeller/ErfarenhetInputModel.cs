using System.ComponentModel.DataAnnotations;

namespace CVSiteGrupp18.Models.CVmodeller
{
    public class ErfarenhetInputModel
    {
        [Required(ErrorMessage = "Företag är obligatoriskt.")]
        [MaxLength(200)]
        public string Företag { get; set; }

        [Required(ErrorMessage = "Roll är obligatoriskt.")]
        [MaxLength(100)]
        public string Roll { get; set; }

        [Required(ErrorMessage = "Beskrivning är obligatoriskt.")]
        public string Beskrivning { get; set; }

        [Required]
        public DateTime Startdatum { get; set; }

        public DateTime? Slutdatum { get; set; }
    }
}
