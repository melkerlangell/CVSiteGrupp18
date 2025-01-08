using Db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace CVSiteGrupp18.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment env;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, Address = model.Address, IsPublic = model.isPublic };
                var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
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
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
				if (result.Succeeded)
				{
					if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
					{
						return Redirect(returnUrl);
					}
					else
					{
						return RedirectToAction("Index", "Home"); ;
					}
				}

                ModelState.AddModelError(string.Empty, "Felaktiga inloggningsuppgifter");
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
            return View();
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
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

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

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
                .Where(u => searchWords.Any(word => u.UserName.Equals(word)) && u.CV.Egenskaper.Any(e => searchWords.Any(word => e.Namn.Equals(word))) || u.UserName.Equals(searchString) || u.CV.Egenskaper.Any(e => e.Namn.Equals(searchString)))
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

        [Authorize]
		[HttpGet]
        public async Task<IActionResult> VisaProfilForAnnanAnvandare(string id)
        {
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

            return View("Profile", user);
        }

    }
}
