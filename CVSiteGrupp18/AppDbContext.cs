using CVSiteGrupp18.Models;
using CVSiteGrupp18.Models.CVmodeller;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CVSiteGrupp18
{
    public class AppDbContext :IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CV> CVs { get; set; }
        public DbSet<Utbildning> Utbildningar { get; set; }
        public DbSet<Erfarenhet> Erfarenheter { get; set; }
        public DbSet<Egenskap> Egenskaper { get; set; }

    }
}
