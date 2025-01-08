using Db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Db;
using System.Security.Claims;

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
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null || message.Reciever != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyMessages");
        }
        public IActionResult Message(string id)
        {
            ViewBag.Id = id;    
            return View();

            
        }
        
        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            
            
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return RedirectToAction("Message", new { id = message.Reciever });
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyMessages()
        {
            var user = await _userManager.GetUserAsync(User);
            var meddelanden = _context.Messages.Where(m => m.Reciever == user.Id).OrderByDescending(m => m.Timestamp).ToList();
            return View(meddelanden);
        }
       
        public IActionResult test(int id)
        {
            return Content("test" + id);
        }
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                
                return NotFound();
            }

            message.isRead = true;
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyMessages");
        }
    }

}
