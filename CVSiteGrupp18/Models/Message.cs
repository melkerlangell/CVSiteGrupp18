using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVSiteGrupp18.Models
{
    public class Message
    {
        public int Id { get; set; }
        //[ForeignKey("User")]
        public string Sender { get; set; }

        [ForeignKey("User")]
        public string Reciever { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool isRead { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }
}
