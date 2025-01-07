using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Db.Models;
using Microsoft.EntityFrameworkCore;

namespace Db.Models.CVmodeller
{
    public class CV
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titel { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Egenskap> Egenskaper { get; set; } = new List<Egenskap>();
        
        public virtual ICollection<Utbildning> Utbildningar { get; set; } = new List<Utbildning>();
        public virtual ICollection<Erfarenhet> Erfarenheter { get; set; } = new List<Erfarenhet>();
    }
}
