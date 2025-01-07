using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Db.Models.CVmodeller
{
    public class Egenskap
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Namn { get; set; }

        [Required]
        [ForeignKey("Cv")]
        public int CvId { get; set; }
        public virtual CV Cv { get; set; }
    }
}
