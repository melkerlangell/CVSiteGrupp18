using CVSiteGrupp18.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CVSiteGrupp18.Controllers
{
    public class MessageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        public MessageController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Message()
        {
            Message message = new Message();
            return View(message);

            
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            Console.WriteLine("test");
            Console.WriteLine(message.Content);
            
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return View("Message");
        }
        [HttpGet]
        public async Task<IActionResult> MyMessages()
        {
            var user = await _userManager.GetUserAsync(User);
            var meddelanden = _context.Messages.Where(m => m.Reciever == user.Id).ToList();
            return View(meddelanden);
        }
    }
}
