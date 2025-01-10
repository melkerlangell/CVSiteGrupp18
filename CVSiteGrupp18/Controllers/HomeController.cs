using System.Diagnostics;
using Db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Db;

namespace CVSiteGrupp18.Controllers
{
    public class HomeController : Controller
    {
		private readonly ILogger<HomeController> _logger;
		private readonly AppDbContext _context;

		public HomeController(ILogger<HomeController> logger, AppDbContext context)
		{
			_logger = logger;
			_context = context;
		}


		[HttpGet]
		public async Task<IActionResult> Index()
		{
			//hämtar 5 cv och 5 projekt
			//blir de 5 nyaste för båda för tar på ID
			var model = new HomeViewModel
			{
				CVs = await _context.CVs.Where(u => u.User.IsPublic).OrderByDescending(c => c.Id).Take(5).ToListAsync(),
				projekt = await _context.Projects.OrderByDescending(p => p.ProjectId).Take(5).ToListAsync()
			};

			return View(model);
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
