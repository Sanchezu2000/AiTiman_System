using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using AiTiman_System.Data;
using AiTIman_System.Areas.Identity.Data;
using System.Net;
using System.Net.Mail;
using AiTIman_System.Data;
using AiTiman_System.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Read MongoDB settings from configuration
var mongoDbConnectionString = builder.Configuration.GetConnectionString("MongoDB");
var databaseName = builder.Configuration["DatabaseSettings:DatabaseName"];


// Register MongoDbContext
builder.Services.AddSingleton<MongoDbContext>(sp => new MongoDbContext(mongoDbConnectionString, databaseName));

// Register IMongoDatabase for dependency injection
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var context = sp.GetRequiredService<MongoDbContext>();
    return context.GetDatabase();
});

// Register custom stores with proper dependency injection for MongoDbContext
builder.Services.AddSingleton<IUserStore<AiTimanUser>, MongoUserStore>();
builder.Services.AddSingleton<IRoleStore<IdentityRole>, MongoRoleStore>();

builder.Services.AddScoped<AppointmentService>();


// Register Identity services
builder.Services.AddIdentity<AiTimanUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // Ensures Identity UI is properly set up

// Register UserProfileService with scoped lifetime
builder.Services.AddScoped<UserProfileService>();

// Configure SmtpClient
builder.Services.AddSingleton(new SmtpClient
{
    Host = "smtp.gmail.com",
    Port = 587,
    Credentials = new NetworkCredential("aitiman.soc@gmail.com", "ekjl fbgd pued hzsm"), // Ensure to use app password
    EnableSsl = true,
});

// Register the IEmailSender service
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// Add logging services
builder.Services.AddLogging();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Default route for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure Identity area routes are handled
app.MapControllerRoute(
    name: "identity",
    pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
   name: "default",
   pattern: "{controller=HealthProvider}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
