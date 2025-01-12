using Db.Models.CVmodeller;
using Db.Models.Projektmodeller;

namespace Db.Models
{
    public class UserProfileDto
    {
        public string UserName { get; set; }
        public string CVTitel { get; set; }
        public List<Egenskap> Egenskaper { get; set; }
        public List<Utbildning> Utbildningar { get; set; }
        public List<Erfarenhet> Erfarenheter { get; set; }
        public List <CreateProject> Projects { get; set; }
    }
}
