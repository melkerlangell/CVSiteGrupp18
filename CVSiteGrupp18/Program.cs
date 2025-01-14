using CVSiteGrupp18;
using Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Db;
using System.Runtime.Serialization;
using CVSiteGrupp18.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//lägger till sql connection med lazy loading. Samt lägger migrations i detta projekt och inte i Db
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
		b => b.MigrationsAssembly("CVSiteGrupp18")));


//skapar user och lägger begränsningar på lösenord
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<XmlSerializerService>();

var app = builder.Build();

// Skapar automatiskt databasen vid start med den senaste migrationen
using (var s = app.Services.CreateScope())
{
	var service = s.ServiceProvider;
	try
	{
		var context = service.GetRequiredService<AppDbContext>();
		context.Database.Migrate();
	}
	catch (Exception ex)
	{
		var logger = service.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "Error vid skapandet av databasen.");
	}
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
