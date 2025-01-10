using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Db.Models.CVmodeller
{
    public class Utbildning
    {
        [Key]
        [XmlIgnore]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vänligen ange skola")]
        [MaxLength(200)]
        public string Skola { get; set; }

        [MaxLength(200)]
        public string Titel { get; set; }

        [Required(ErrorMessage = "Vänligen ange startdatum")]
        public DateTime StartDatum { get; set; }

        public DateTime? SlutDatum { get; set; }

        [Required]
        [ForeignKey("Cv")]
        [XmlIgnore]
        public int CvId { get; set; }
        public virtual CV Cv { get; set; }
    }
}
