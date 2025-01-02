using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CVSiteGrupp18.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        [PersonalData]
        public string Address { get; set; }
        [PersonalData]
        public bool IsPublic { get; set; }


        public string? ProfilePicture { get; set; } = "default.jpg";

        public virtual CVSiteGrupp18.Models.CVmodeller.CV? CV { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

       



    }
}
