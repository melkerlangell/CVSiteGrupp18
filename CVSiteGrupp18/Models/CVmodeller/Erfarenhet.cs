using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CVSiteGrupp18.Models.CVmodeller
{
    public class Erfarenhet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Arbetsplats { get; set; }

        [Required]
        [MaxLength(100)]
        public string Roll { get; set; }

        public string Beskrivning { get; set; }

        [Required]
        public DateTime StartDatum { get; set; }

        public DateTime? SlutDatum { get; set; } 

        [Required]
        [ForeignKey("Cv")]
        public int CvId { get; set; }
        public virtual CV Cv { get; set; }
    }
}
