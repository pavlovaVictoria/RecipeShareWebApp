
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using RecipeShare.Repositories.Interfaces;
using RecipeShare.Repositories;
using RecipeShare.Services.Data;
using RecipeShare.Services.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = true);

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
        policy.RequireRole("User", "Administrator"))
    .AddPolicy("CanApproveRecipes", policy =>
        policy.RequireRole("Moderator", "Administrator"))
    .AddPolicy("CanManageEverything", policy =>
        policy.RequireRole("Administrator"))
    .AddPolicy("AccountSettings", policy =>
        policy.RequireRole("User", "Administrator", "Moderator"));

builder.Services
    //Register Repositories
    .AddScoped<IRecipeRepository, RecipeRepository>()
    .AddScoped<ICommentRepository, CommentRepository>()
    .AddScoped<IAccountRepository, AccountRepository>()
    //Register Services
    .AddScoped<IAccountService, AccountService>()
    .AddScoped<IRecipeService, RecipeService>()
    .AddScoped<IHomeService, HomeService>()
    .AddScoped<ICommentService, CommentService>()
    .AddScoped<IAdministratorService, AdministratorService>()
    .AddScoped<IModeratorService, ModeratorService>()
    .AddScoped<IAccountSettingsService, AccountSettingsService>();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

var app = builder.Build();

await DataSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/500");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseExceptionHandler("/Error/500");
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id:guid?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id:guid?}");
app.MapRazorPages();

app.Run();
