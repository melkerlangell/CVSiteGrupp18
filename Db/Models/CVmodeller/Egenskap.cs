using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Db.Models.CVmodeller
{
    public class Egenskap
    {
        [Key]
        [XmlIgnore]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Namn { get; set; }

        [Required]
        [ForeignKey("Cv")]
        [XmlIgnore]
        public int CvId { get; set; }
        public virtual CV Cv { get; set; }
    }
}
