using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Db.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        [PersonalData]
        public string Address { get; set; }
        [PersonalData]
        public bool IsPublic { get; set; }


        public string? ProfilePicture { get; set; } = "default.jpg";

        public virtual Db.Models.CVmodeller.CV? CV { get; set; }
        [XmlIgnore]
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

        [InverseProperty("User")]
        [XmlIgnore]
        public virtual ICollection<ProjektUser> ProjektUsers { get; set; }

        public bool IsActive { get; set; } = true;





    }
}
