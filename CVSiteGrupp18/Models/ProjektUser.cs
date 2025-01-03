﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CVSiteGrupp18.Models.Projektmodeller;

namespace CVSiteGrupp18.Models
{
    public class ProjektUser
    {
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual CreateProject Projekt { get; set; }

        
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}