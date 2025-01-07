using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Db.Models.CVmodeller
{
    public class Utbildning
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Skola { get; set; }

        [MaxLength(200)]
        public string Titel { get; set; }

        [Required]
        public DateTime StartDatum { get; set; }

        public DateTime? SlutDatum { get; set; }

        [Required]
        [ForeignKey("Cv")]
        public int CvId { get; set; }
        public virtual CV Cv { get; set; }
    }
}
