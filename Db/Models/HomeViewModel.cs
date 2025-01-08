namespace Db.Models
{
	public class HomeViewModel
	{
		public List<CVmodeller.CV> CVs { get; set; }
		public List<Projektmodeller.CreateProject> projekt { get; set; }

		public string User { get; set; }
    }
}
