using System.Diagnostics;
using CVSiteGrupp18.Models;
using CVSiteGrupp18.Models.Projektmodeller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CVSiteGrupp18.Controllers
{
    public class ProjectController : Controller
    {
        [HttpGet]
       public IActionResult SkapaProjekt()
        {
            var viewModel = new CreateProject();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SkapaProjekt(CreateProject model)
        {
            if(ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        
    }
}
