using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Models
{
    public class DeactivateAccountViewModel
    {
        public string UserId { get; set; }
        public bool ConfirmDeactivation { get; set; }
    }
}
