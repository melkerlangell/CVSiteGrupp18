﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using Azure.Core;
using Db.Models;
using Db.Models.Projektmodeller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Db;


namespace CVSiteGrupp18.Controllers
{
    //kontroller för allt som har med projekt att göra
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

            //lägger till http ifall bifogade länken saknar det
            if(model.ExternalLink != null)
            {
                if (model.ExternalLink.Contains("http://") || model.ExternalLink.Contains("https://"))
                {
                    model.ExternalLink = model.ExternalLink;
                }
                else
                {
                    model.ExternalLink = "http://" + model.ExternalLink;
                }
            }

            
            //lägger till projekt
            _context.Projects.Add(model);
            await _context.SaveChangesAsync();

            //skapar en ny instans i sambandstabellen
            var projectUser = new ProjektUser
            {
                ProjectId = model.ProjectId,                  
                UserId = user.Id              
            };

            _context.ProjektUsers.Add(projectUser);
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

            //hämtar projekt som nuvarande användare skapat eller gåt med i
            var projects = await _context.Projects
                .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)
                .Where(p => p.UserId == user.Id || p.ProjectUsers.Any(pu => pu.UserId == user.Id))
                .ToListAsync();

            return View(projects);
        }

        [HttpGet]
        public async Task<IActionResult> RedigeraProjekt(int id)
        {
            //hämtar projekt på parameter id via asp-route-id
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if(project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        
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

                    //uppdaterar med inmatningsdata
                    existingProject.Title = model.Title;
                    existingProject.Description = model.Description;
                    existingProject.StartDatum = model.StartDatum;
                    existingProject.SlutDatum = model.SlutDatum;
                    existingProject.ExternalLink = model.ExternalLink;

                    //sparar
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
        
        public async Task<IActionResult> TaBortProjekt(int id)
        {
            //hittar projekt med asp-route-id id
            var projekt = await _context.Projects.FindAsync(id);
            if (projekt == null) 
            {
                return NotFound();
            }

            //tar bort
            _context.Projects.Remove(projekt);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MinaProjekt));
        }


        [HttpGet]
        public async Task<IActionResult> AllaProjekt()
        {
            //hämtar alla projekt
            var projects = await _context.Projects.OrderByDescending(u => u.ProjectId).ToListAsync();
            return View(projects);
        }

        [HttpGet]
        [Authorize]
		public async Task<IActionResult> DetaljerSpecifiktProjekt(int projectId)
        {
            //hämtar projektdata inkl data om de användare som är knutna till projektet
            var project = await _context.Projects
                .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)  // Inkludera användaren som är med i projektet
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null)
            {
                return NotFound();
            }

            //checkar så alla som är med är aktiva
            project.ProjectUsers = project.ProjectUsers.Where(u => u.User.IsActive).ToList();

            return View(project);  // Skicka projektet till vyn
        }



        [HttpPost]
        public async Task<IActionResult> GåMedProjekt(int projectId)
        {
            var user = await _userManager.GetUserAsync(User);  // Hämtar den inloggade användaren
            var project = await _context.Projects.FindAsync(projectId);

            if (project == null)
            {
                return NotFound();
            }

            // Kontrollera om användaren redan är med i projektet
            var existingProjectUser = await _context.ProjektUsers
                .FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == user.Id);

            if (existingProjectUser != null)
            {
                return View("RedanMedIProjekt");
            }

            // Lägg till användaren i projektet
            var projectUser = new ProjektUser
            {
                ProjectId = projectId,
                UserId = user.Id
            };

            _context.ProjektUsers.Add(projectUser);
            await _context.SaveChangesAsync();


            return RedirectToAction("DetaljerSpecifiktProjekt", new { projectId = projectId });


        }

        [HttpPost]
        public async Task<IActionResult> LämnaProjekt(int projectId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var projectUser = await _context.ProjektUsers
                .FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == user.Id);

            if (projectUser == null)
            {
                return NotFound("Du är inte med i projektet.");
            }

            //tar bort användaren från projektet
            _context.ProjektUsers.Remove(projectUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("MinaProjekt");
        }


		[HttpGet]
		public async Task<IActionResult> VisaProjektForAnnanAnvandare(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			var projects = await _context.Projects
				.Include(p => p.ProjectUsers)
				.ThenInclude(pu => pu.User)
				.Where(p => p.UserId == id || p.ProjectUsers.Any(pu => pu.UserId == id))
				.ToListAsync();

			return View(projects);
		}


	}
}
