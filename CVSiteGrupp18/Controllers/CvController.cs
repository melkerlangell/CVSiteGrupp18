using CVSiteGrupp18.Models;
using CVSiteGrupp18.Models.CV.CV;
using CVSiteGrupp18.Models.CVmodeller;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVSiteGrupp18.Controllers
{
    public class CvController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public CvController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult SkapaCv()
        {
            var viewModel = new CreateCvViewModel();
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SkapaCv(CreateCvViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                var existingCV = await _context.CVs
                                   .Where(c => c.UserId == user.Id)
                                   .FirstOrDefaultAsync();

                if (existingCV != null)
                {
                    ModelState.AddModelError(string.Empty, "Enbart ett CV per användare, du kan ändra ditt befintliga");
                    return View(model);
                }


                var newCv = new CV
                {
                    Titel = model.Titel,
                    UserId = user.Id,
                    Egenskaper = new List<Egenskap>(),
                    Utbildningar = new List<Utbildning>(),
                    Erfarenheter = new List<Erfarenhet>()
                };


                foreach (var kompetensNamn in model.Kompetenser)
                {
                    newCv.Egenskaper.Add(new Egenskap { Namn = kompetensNamn});
                }


                foreach (var utbildning in model.Utbildningar)
                {
                    newCv.Utbildningar.Add(new Utbildning
                    {
                        Skola = utbildning.Skola,
                        Titel = utbildning.Titel,
                        StartDatum = utbildning.Startdatum,
                        SlutDatum = utbildning.Slutdatum
                    });
                }


                foreach (var erfarenhet in model.Erfarenheter)
                {
                    newCv.Erfarenheter.Add(new Erfarenhet
                    {
                        Arbetsplats = erfarenhet.Företag,
                        Roll = erfarenhet.Roll,
                        Beskrivning = erfarenhet.Beskrivning,
                        StartDatum = erfarenhet.Startdatum,
                        SlutDatum = erfarenhet.Slutdatum
                    });
                }


                _context.CVs.Add(newCv);
                await _context.SaveChangesAsync();

                return RedirectToAction("CvDetaljer");
            }
        }

        public async Task<IActionResult> CvDetaljer()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // Om användaren inte är inloggad
            }

            var cv = await _context.CVs
                .Include(c => c.Egenskaper)
                .Include(c => c.Utbildningar)
                .Include(c => c.Erfarenheter)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cv == null)
            {
                // Omdirigera till sidan för att skapa CV ifall cv saknas
                return RedirectToAction("SkapaCv");
            }

            return View(cv);
        }

    }
}

