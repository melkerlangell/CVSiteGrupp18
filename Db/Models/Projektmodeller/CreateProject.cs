﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Db.Models.Projektmodeller
{
    public class CreateProject
    {
        [Key]
        [XmlIgnore]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Titel är obligatorisk")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Beskrivning är obligatorisk")]
        public string Description { get; set; }

        public string? ExternalLink { get; set; }

        [Required(ErrorMessage = "Startdatum är obligatorisk")]
        public DateTime? StartDatum { get; set; }
        public DateTime? SlutDatum { get; set; }


        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        [InverseProperty("Projekt")]
        [XmlIgnore]
        public virtual ICollection<ProjektUser>? ProjectUsers { get; set; }
    }
}
