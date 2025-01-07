using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Db.Models.Projektmodeller;

namespace Db.Models
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
