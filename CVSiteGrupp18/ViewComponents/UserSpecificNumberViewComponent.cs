using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Db.Models;
using System.Threading.Tasks;
using Db;

namespace CVSiteGrupp18.ViewComponents
{
    public class UserSpecificNumberViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public UserSpecificNumberViewComponent(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(UserClaimsPrincipal);
            if (user == null)
            {
                return Content("");
            }

            // Här kan du beräkna den dynamiska siffran för användaren
            var userSpecificNumber = _context.Messages.Count(m => m.Reciever == user.Id && !m.isRead);
            if (userSpecificNumber == 0)
            {
                return Content("");
            }
            return View(userSpecificNumber);
        }
    }
}
