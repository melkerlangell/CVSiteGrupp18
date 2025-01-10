using System.ComponentModel.DataAnnotations;

namespace Db.Models.CVmodeller
{
    public class UtbildningInputModel
    {
        [Required(ErrorMessage = "Lärosäte är obligatoriskt.")]
        [MaxLength(200)]
        public string Skola { get; set; }

        [MaxLength(200)]
        public string Titel { get; set; }

        [Required(ErrorMessage = "Vänligen ange startdatum")]
        public DateTime Startdatum { get; set; }

        public DateTime? Slutdatum { get; set; }
    }
}
