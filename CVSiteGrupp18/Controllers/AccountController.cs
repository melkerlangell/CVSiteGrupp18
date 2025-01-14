using CVSiteGrupp18.Services;
using Db.Models;
using Db.Models.CVmodeller;
using Db.Models.Projektmodeller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace CVSiteGrupp18.Controllers
{
    //controller för allt som har med ApplicationUser och identity att göra
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly XmlSerializerService _xmlSerializerService;
        //används för att skriva över profilbilder till wwwroot
        private readonly IWebHostEnvironment env;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, XmlSerializerService xmlSerializerService, IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _xmlSerializerService = xmlSerializerService;
            this.env = env;
        }

		[HttpGet]
		public IActionResult Register(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}


		[HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                //skapar ny user
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, Address = model.Address, IsPublic = model.isPublic, PhoneNumber = model.TelefonNummer };

                //skapar user
                var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
                    //loggar in som nya usern ifall registreing lyckades
					await _signInManager.SignInAsync(user, isPersistent: false);

					if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
					{
						return Redirect(returnUrl);
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}

				foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

			ViewData["ReturnUrl"] = returnUrl;
			return View(model);
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
			ViewData["ReturnUrl"] = returnUrl;
			return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LogInViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                //kontrollerar att det finns användare med det användarnamnet
                var user = await _userManager.FindByNameAsync(model.UserName);

                //kontroll för deaktiverat konto
                if (user != null && !user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Ditt konto är inaktiverat.");
                    return View(model);
                }

                //försöker logga in med angivna uppgifter
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }


                ModelState.AddModelError(string.Empty, "Felaktiga inloggningsuppgifter.");
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        

        [HttpGet]
        public IActionResult EditProfile()
        {
            //fyller i befintliga uppgifter

            var user = _userManager.GetUserAsync(User);
            var userEdit = new EditProfileViewModel
			{
				UserName = user.Result.UserName,
				Email = user.Result.Email,
				Address = user.Result.Address,
				IsPublic = user.Result.IsPublic,
                TelefonNummer = user.Result.PhoneNumber
			};


			return View(userEdit);
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
           //ifall något är ändrat och inte tomt så uppdateras det
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(model.UserName) && model.UserName != user.UserName)
            {
                user.UserName = model.UserName;
            }

            if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != user.Email)
            {
                user.Email = model.Email;
            }

            if (!string.IsNullOrWhiteSpace(model.TelefonNummer) && model.TelefonNummer != user.PhoneNumber)
            {
                user.PhoneNumber = model.TelefonNummer;
            }

            if (!string.IsNullOrWhiteSpace(model.Address) && model.Address != user.Address)
            {
                user.Address = model.Address;
            }

            if (model.IsPublic != user.IsPublic)
            {
                user.IsPublic = model.IsPublic;
            }



            if (model.ProfilBild != null)
            {
                //kontroll att profilbild är jpg, jpeg eller png för annat verkar inte fugnera
                string ext = Path.GetExtension(model.ProfilBild.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                {
                    ModelState.AddModelError("ProfilBild", "Endast .jpg, .jpeg och .png filer är tillåtna.");
                    return View(model);
                }

                string nyFilNamn = user.Id + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                nyFilNamn += ext;

                string bildFullSok = env.WebRootPath + "/Profilepictures/" + nyFilNamn;

                using (var stream = System.IO.File.Create(bildFullSok))
                {
                    model.ProfilBild.CopyTo(stream);
                }

                user.ProfilePicture = nyFilNamn;
            }


            //ändring av lösenord
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Nuvarande lösenord krävs för att ändra lösenord.");
                }
                else if (model.NewPassword != model.ConfirmNewPassword)
                {
                    ModelState.AddModelError("ConfirmNewPassword", "Det nya lösenordet stämmer inte överens med det bekräftade lösenordet");
                }
                else
                {
                    var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (!passwordChangeResult.Succeeded)
                    {
                        foreach (var error in passwordChangeResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //uppdaterar användaren
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            //loggar in på nytt med de nya uppgifterna
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Profile", "Account");
        }

        public IActionResult SearchUser()
        {
            return View();
        }


        // tar in ett användarnamn från ett input och kollar om användaren finns & hämtar den
        [HttpGet]
        public async Task<IActionResult> SearchUsers(string searchString)
        {
            // Kontrollerar om söksträngen är null eller bara innehåller blanksteg
            if (string.IsNullOrWhiteSpace(searchString))
            {
                ViewBag.Error = "Vänligen ange ett sökord.";
                return View("SearchUser", null);
            }

            // Delar upp söksträngen i en array av ord, delade vid varje blanksteg
            string[] searchWords = searchString.Split(' ');

            // Hämtar den inloggade användaren
            var currentUser = await _userManager.GetUserAsync(User);

            // Hämtar användare vars användarnamn eller kompetenser matchar något av sökorden (kompetenser heter egenskaper i databasen)
            var users = await _userManager.Users
                .Include(u => u.CV)
                .ThenInclude(cv => cv.Utbildningar)
            .Include(u => u.CV)
                .ThenInclude(cv => cv.Erfarenheter)
            .Include(u => u.CV)
                .ThenInclude(cv => cv.Egenskaper)
                .Where(u => u.IsActive && (searchWords.Any(word => u.UserName.Contains(word)) && u.CV.Egenskaper.Any(e => searchWords.Any(word => e.Namn.Equals(word))) || u.UserName.Contains(searchString) || u.CV.Egenskaper.Any(e => e.Namn.Equals(searchString))))
                .ToListAsync();

            // Om en inloggad användare söker exkluderas den från sökresultatet
            if (currentUser != null)
            {
                users = users.Where(u => u.Id != currentUser.Id).ToList();
            }

            //kollar ifall det inte finns några användare
            if (users == null || users.Count == 0)
            {
                ViewBag.Error = "Hittade inga profiler med den sökningen.";
                return View("SearchUser", null);
            }

            // filtrerar att enbart visa publika användare ifall användaren som gör sökningen inte är registrerad
            if (!User.Identity.IsAuthenticated)
            {
                users = users.Where(u => u.IsPublic).ToList();
            }

            // Om man inte är inloggad och den profil man söker efter inte är offentlig visas ett felmeddelande
            if (users.Count == 0)
            {
                ViewBag.Error = "Inga profiler att visa. De kan vara privata.";
                return View("SearchUser", null);
            }

            return View("SearchUser", users);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        //[Authorize]
		[HttpGet]
        public async Task<IActionResult> VisaProfilForAnnanAnvandare(string id)
        {
            //hämtar användare på användarid med asp-route-id
            var user = await _userManager.Users
                .Include(u => u.CV)
                .ThenInclude(cv => cv.Utbildningar)
                .Include(u => u.CV)
                .ThenInclude(cv => cv.Erfarenheter)
                .Include(u => u.CV)
                .ThenInclude(cv => cv.Egenskaper)
                .FirstOrDefaultAsync(u => u.Id == id);


            if (user == null)
            {
                return NotFound();
            }

            if (!user.IsPublic && !User.Identity.IsAuthenticated)
            {
                return Forbid();
            }

            if (!user.IsActive)
            {
                return Forbid();
            }

            return View("Profile", user);
        }


        [HttpGet]
        public IActionResult DeactivateAccount()
        {
            var model = new DeactivateAccountViewModel
            {
                UserId = _userManager.GetUserId(User)
            };
            return View(model);
        }

        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeactivateAccount(DeactivateAccountViewModel model)
        {
            if (!model.ConfirmDeactivation)
            {
                ModelState.AddModelError(string.Empty, "Bekräfta avaktivering.");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.SignOutAsync();

            TempData["Message"] = "Ditt konto har avaktiverats. Du kan inte logga in längre.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ExportProfile(string id)
        {
            // Hämtar användaren och väsentlig information baserat på id:t från användarmanagern
            var user = await _userManager.Users
                .Include(u => u.CV)
                .ThenInclude(cv => cv.Utbildningar)
                .Include(u => u.CV)
                .ThenInclude(cv => cv.Erfarenheter)
                .Include(u => u.CV)
                .ThenInclude(cv => cv.Egenskaper)
                .Include(u => u.ProjektUsers)
                .ThenInclude(pu => pu.Projekt)
                .FirstOrDefaultAsync(u => u.Id == id);

            // Om användaren inte hittas, returneras en "Not Found" med ett felmeddelande
            if (user == null)
            {
                return NotFound("Profilen hittades inte.");
            }

            // Om användaren inte har ett CV, returneras ett felmeddelande om att CV saknas
            if (user.CV == null)
            {
                return NotFound("Profilen har inget CV.");
            }

            // Skapar ett DTO-objekt där användarens data mappas för att inte exponera känslig information
            // och "plattas ut" för att kunna hantera problem med proxyobjekt (Lazy Loading)
            var userProfileDto = new UserProfileDto
            {
                UserName = user.UserName,
                CVTitel = user.CV.Titel,
                Egenskaper = user.CV.Egenskaper.Select(e => new Egenskap
                {
                    Id = e.Id,
                    Namn = e.Namn,
                }).ToList(),
                Utbildningar = user.CV.Utbildningar.Select(u => new Utbildning
                {
                    Id = u.Id,
                    Skola = u.Skola,
                    Titel = u.Titel,
                    StartDatum = u.StartDatum,
                    SlutDatum = u.SlutDatum,
                }).ToList(),
                Erfarenheter = user.CV.Erfarenheter.Select(e => new Erfarenhet
                {
                    Id = e.Id,
                    Arbetsplats = e.Arbetsplats,
                    Roll = e.Roll,
                    Beskrivning = e.Beskrivning,
                    StartDatum = e.StartDatum,
                    SlutDatum = e.SlutDatum,
                }).ToList(),
                Projects = user.ProjektUsers.Select(pu => new CreateProject
                {
                    ProjectId = pu.Projekt.ProjectId,
                    Title = pu.Projekt.Title,
                    Description = pu.Projekt.Description,
                    ExternalLink = pu.Projekt.ExternalLink,
                    StartDatum = pu.Projekt.StartDatum,
                    SlutDatum = pu.Projekt.SlutDatum,
                }).ToList()
            };

            // Skapar sökvägen där XML-filen ska sparas (i webbroten)
            var filePath = Path.Combine(env.WebRootPath, "profile.xml");

            // Serialiserar användarens DTO till XML och sparar det till fil
            _xmlSerializerService.Serialize(userProfileDto, filePath);

            // Öppnar en filström för att läsa den nyligen skapade XML-filen
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Returnerar filen som en nedladdningsbar XML-fil
            return File(fileStream, "application/xml", "profile.xml");
        }
    }

}

