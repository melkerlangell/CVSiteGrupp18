using System.ComponentModel.DataAnnotations;
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

        [PersonalData]
        public string? ProfilePicture { get; set; }
    }
}
