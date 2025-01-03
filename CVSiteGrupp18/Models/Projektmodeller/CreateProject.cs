using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVSiteGrupp18.Models.Projektmodeller
{
    public class CreateProject
    {
        [Key]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Titel är obligatorisk")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Beskrivning är obligatorisk")]
        public string Description { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [InverseProperty("Projekt")]
        public virtual ICollection<ProjektUser>? ProjectUsers { get; set; }
    }
}
