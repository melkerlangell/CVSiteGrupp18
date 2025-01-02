using System.Diagnostics;
using System.Runtime.CompilerServices;
using Azure.Core;
using CVSiteGrupp18.Models;
using CVSiteGrupp18.Models.Projektmodeller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            model.UserId = user.Id; 

            _context.Projects.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("MinaProjekt");
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

        [HttpGet]
        public async Task<IActionResult> RedigeraProjekt(int id)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if(project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RedigeraProjekt(int id, CreateProject model)
        {
            if (id != model.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProject = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == id);
                    if (existingProject == null)
                    {
                        return NotFound();
                    }

                    
                    existingProject.Title = model.Title;
                    existingProject.Description = model.Description;

                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Projects.Any(p => p.ProjectId == model.ProjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(MinaProjekt));
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaBortProjekt(int id)
        {
            var projekt = await _context.Projects.FindAsync(id);
            if (projekt == null) 
            {
                return NotFound();
            }

            _context.Projects.Remove(projekt);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MinaProjekt));
        }
    }
}
