using Db.Models.CVmodeller;
using Db.Models.Projektmodeller;

namespace Db.Models
{
    public class UserProfileDto
    {
        public string UserName { get; set; }
        public string CVTitel { get; set; }
        public List<Egenskap> Egenskaper { get; set; } = new List<Egenskap>();
        public List<Utbildning> Utbildningar { get; set; } = new List<Utbildning>();
        public List<Erfarenhet> Erfarenheter { get; set; } = new List<Erfarenhet>();
        public List <CreateProject> Projects { get; set; } = new List<CreateProject>();
    }
}
