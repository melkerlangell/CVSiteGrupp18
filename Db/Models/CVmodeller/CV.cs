using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Db.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace Db.Models.CVmodeller
{
    public class CV
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titel { get; set; }


        public int AntalVisningar { get; set; } = 0;    


		[Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        [XmlIgnore]
        public virtual ApplicationUser User { get; set; }
        [XmlIgnore]
        public virtual ICollection<Egenskap> Egenskaper { get; set; } = new List<Egenskap>();
        [XmlIgnore]
        public virtual ICollection<Utbildning> Utbildningar { get; set; } = new List<Utbildning>();
        [XmlIgnore]
        public virtual ICollection<Erfarenhet> Erfarenheter { get; set; } = new List<Erfarenhet>();
    }
}
