
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Services.Data;
using RecipeShare.Services.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<RecipeShareDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
  .AddEntityFrameworkStores<RecipeShareDbContext>()
  .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanViewAndAddRecipes", policy =>
        policy.RequireRole("User", "Moderator", "Administrator"))
    .AddPolicy("CanApproveRecipes", policy =>
        policy.RequireRole("Moderator", "Administrator"))
    .AddPolicy("CanManageEverything", policy =>
        policy.RequireRole("Administrator"));

builder.Services
    .AddScoped<IAccountService, AccountService>();

var app = builder.Build();

await DataSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/500"); // Custom page for server errors.
    app.UseStatusCodePagesWithReExecute("/Error/{0}"); // Custom pages for status codes like 404 or 403.
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
