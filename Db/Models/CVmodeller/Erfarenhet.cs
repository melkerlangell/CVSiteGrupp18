using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Db.Models.CVmodeller
{
    public class Erfarenhet
    {
        [Key]
        [XmlIgnore]
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
        [XmlIgnore]
        public int CvId { get; set; }
        public virtual CV Cv { get; set; }
    }
}
