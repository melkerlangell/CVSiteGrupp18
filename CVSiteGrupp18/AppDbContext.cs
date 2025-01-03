using CVSiteGrupp18.Models;
using CVSiteGrupp18.Models.CVmodeller;
using CVSiteGrupp18.Models.Projektmodeller;
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
        public DbSet<CreateProject> Projects { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ProjektUser> ProjektUsers { get; set; }




        //för att kunna skapa sambandstabellen för projekt och användare
        //fungerade inte med data annotations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Definiera den sammansatta primärnyckeln för ProjectUser
            modelBuilder.Entity<ProjektUser>()
                .HasKey(pu => new { pu.ProjectId, pu.UserId });

            // Definiera relationen mellan ProjectUser och Project
            modelBuilder.Entity<ProjektUser>()
                .HasOne(pu => pu.Projekt)
                .WithMany(p => p.ProjectUsers)
                .HasForeignKey(pu => pu.ProjectId);

            // Definiera relationen mellan ProjectUser och ApplicationUser
            modelBuilder.Entity<ProjektUser>()
                .HasOne(pu => pu.User)
                .WithMany(u => u.ProjektUsers)
                .HasForeignKey(pu => pu.UserId);
        }

    }
}
