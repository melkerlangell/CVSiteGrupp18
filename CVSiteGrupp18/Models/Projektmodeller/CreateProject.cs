using System.ComponentModel.DataAnnotations;

namespace CVSiteGrupp18.Models.Projektmodeller
{
    public class CreateProject
    {
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Titel är obligatorisk")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Beskrivning är obligatorisk")]
        public string Description { get; set; }
    }
}
