using System.Diagnostics;
using System.Runtime.CompilerServices;
using Azure.Core;
using CVSiteGrupp18.Models;
using CVSiteGrupp18.Models.Projektmodeller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace CVSiteGrupp18.Controllers
{
    public class ProjectController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public ProjectController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult SkapaProjekt()
        {
            var viewModel = new CreateProject();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SkapaProjekt(CreateProject model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }
                model.UserId = user.Id;

                _context.Projects.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");

            }
            return View(model);

        }

        
        public async Task<IActionResult> MinaProjekt()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var projects = _context.Projects.Where(p => p.UserId == user.Id).ToList();
            return View(projects);
        }

    }
}
