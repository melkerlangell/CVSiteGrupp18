
using CVSiteGrupp18.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVSiteGrupp18.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, Address = model.Address, IsPublic = model.isPublic };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("UserLandingPage", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LogInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("UserLandingPage", "Account");
                }

                ModelState.AddModelError(string.Empty, "Kunde inte logga in");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult UserLandingPage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            return View();
        }



        [HttpPost]
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
            return RedirectToAction("UserLandingPage", "Account");
        }

        public IActionResult SearchUser()
        {
            return View();
        }


        // tar in ett användarnamn från ett input och kollar om användaren finns & hämtar den
        [HttpGet]
        public async Task<IActionResult> SearchUsers(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ViewBag.Error = "Ange ett användarnamn att söka efter.";
                return View("SearchUser", null);
            }

            // Användarnamn som innehåller sökningen
            var users = await _userManager.Users
                .Where(u => u.UserName.Contains(username))
                .ToListAsync();


            //kollar ifall det inte finns några användare
            if (users == null || users.Count == 0)
            {
                ViewBag.Error = "Hittar inte några användare med det namnet.";
                return View("SearchUser", null);
            }

            // filtrerar att enbart visa publika användare ifall användaren som gör sökningen inte är registrerad
            if (!User.Identity.IsAuthenticated)
            {
                users = users.Where(u => u.IsPublic).ToList();
            }

            if (users.Count == 0)
            {
                ViewBag.Error = "Inga profiler att visa. De kan vara privata.";
                return View("SearchUser", null);
            }

            return View("SearchUser", users);
        }

    }
}
