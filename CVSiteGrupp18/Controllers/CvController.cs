using Db.Models;
using Db.Models.CV.CV;
using Db.Models.CVmodeller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Db;

namespace CVSiteGrupp18.Controllers
{
    //kontroller för allt som har med cv att göra
    public class CvController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public CvController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult SkapaCv()
        {
            var viewModel = new CreateCvViewModel();
            
            return View(viewModel);
        }



        //skapacv används som post metod både vid skapandet och redigerandet
        [HttpPost]
        public async Task<IActionResult> SkapaCv(CreateCvViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            //kontrollerar ifall användaren har ett cv
            var existingCV = await _context.CVs
                                           .Include(c => c.Egenskaper)
                                           .Include(c => c.Utbildningar)
                                           .Include(c => c.Erfarenheter)
                                           .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (existingCV != null)
            {
                // Om ett CV redan finns, uppdatera det
                existingCV.Titel = model.Titel;

                // Rensa befintliga egenskaper, utbildningar och erfarenheter
                _context.Egenskaper.RemoveRange(existingCV.Egenskaper);
                _context.Utbildningar.RemoveRange(existingCV.Utbildningar);
                _context.Erfarenheter.RemoveRange(existingCV.Erfarenheter);

                // Lägg till nya egenskaper
                foreach (var kompetensNamn in model.Kompetenser)
                {
                    if (!string.IsNullOrWhiteSpace(kompetensNamn))
                    {
                        existingCV.Egenskaper.Add(new Egenskap { Namn = kompetensNamn });
                    }
                }

                // Lägg till nya utbildningar
                foreach (var utbildning in model.Utbildningar)
                {
                    existingCV.Utbildningar.Add(new Utbildning
                    {
                        Skola = utbildning.Skola,
                        Titel = utbildning.Titel,
                        StartDatum = utbildning.Startdatum,
                        SlutDatum = utbildning.Slutdatum
                    });
                }

                // Lägg till nya erfarenheter
                foreach (var erfarenhet in model.Erfarenheter)
                {
                    existingCV.Erfarenheter.Add(new Erfarenhet
                    {
                        Arbetsplats = erfarenhet.Företag,
                        Roll = erfarenhet.Roll,
                        Beskrivning = erfarenhet.Beskrivning,
                        StartDatum = erfarenhet.Startdatum,
                        SlutDatum = erfarenhet.Slutdatum
                    });
                }

                _context.CVs.Update(existingCV);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Om inget CV finns, skapa ett nytt
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
                    if (!string.IsNullOrWhiteSpace(kompetensNamn))
                    {
                        newCv.Egenskaper.Add(new Egenskap { Namn = kompetensNamn });
                    }
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
            }

            return RedirectToAction("Profile","Account");
        }


        public async Task<IActionResult> CvDetaljer(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            //hämtar cv för den profil man är inne på
            var cv = await _context.CVs
                .Include(c => c.User) 
                .Include(c => c.Egenskaper)
                .Include(c => c.Utbildningar)
                .Include(c => c.Erfarenheter)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            //ifall det inte finns något cv
            if (cv == null)
            {
                if(currentUser == null)
                {
                    return View("SaknarCv");
                }

                if (currentUser.Id == userId)
                {
                    return RedirectToAction("SkapaCv");
                }

                return View("SaknarCv");
            }


            //ökar bara antalet visningar ifall någon annan besöker ens cv
            if (currentUser == null || currentUser.Id != userId)
            {
				await ÖkaAntalVisningar(cv.Id);
			}
				


			return View(cv);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteCv(int cvId)
        {
            //tar nuvarande användare
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            //hittar cv för användare
            var cv = await _context.CVs.FirstOrDefaultAsync(c => c.UserId == user.Id && c.Id == cvId);  
            if (cv == null)
            {
                return NotFound();
            }

            //tar bort cv
            _context.CVs.Remove(cv);
            await _context.SaveChangesAsync();  

            return RedirectToAction("Profile", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> RedigeraCv()
        {
            //hämtar all information kopplat till cv
            //ändringar sker sedan via skapacv
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var existingCV = await _context.CVs
                                           .Include(c => c.Egenskaper)
                                           .Include(c => c.Utbildningar)
                                           .Include(c => c.Erfarenheter)
                                           .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (existingCV == null)
            {
                return RedirectToAction("SkapaCv"); // Om inget CV finns, skapar vi ett nytt
            }

            var viewModel = new CreateCvViewModel
            {
                Titel = existingCV.Titel,
                Kompetenser = existingCV.Egenskaper.Select(e => e.Namn).ToList(),
                Utbildningar = existingCV.Utbildningar.Select(u => new UtbildningInputModel
                {
                    Skola = u.Skola,
                    Titel = u.Titel,
                    Startdatum = u.StartDatum,
                    Slutdatum = u.SlutDatum
                }).ToList(),
                Erfarenheter = existingCV.Erfarenheter.Select(e => new ErfarenhetInputModel
                {
                    Företag = e.Arbetsplats,
                    Roll = e.Roll,
                    Beskrivning = e.Beskrivning,
                    Startdatum = e.StartDatum,
                    Slutdatum = e.SlutDatum
                }).ToList()
            };

            return View("SkapaCv", viewModel); // Använd samma vy som för att skapa CV
        }


		public async Task ÖkaAntalVisningar(int cvId)
		{
			var cv = await _context.CVs.FirstOrDefaultAsync(c => c.Id == cvId);

			if (cv != null)
			{
				cv.AntalVisningar++;
				await _context.SaveChangesAsync();
			}
		}


		[HttpGet]
		public async Task<IActionResult> MatchaCvMedAndra(string userId)
		{
            //hämtar  cv för nuvarande profil
			var userCv = await _context.CVs
	            .Include(c => c.User)
	            .Include(c => c.Egenskaper)
	            .FirstOrDefaultAsync(c => c.UserId == userId);

			if (userCv == null)
			{
				return View("SaknarCv");
			}

            //hittar cv med samma egenskap/er som det vars profil man är inne på
			var match = await _context.CVs
				.Include(c => c.User)
				.Include(c => c.Egenskaper)
				.Where(c => c.UserId != userId && c.User.IsActive &&
							c.Egenskaper.Any(e => userCv.Egenskaper.Select(ue => ue.Namn).Contains(e.Namn)))
				.ToListAsync();

			return View(match);
		}

	}
}

