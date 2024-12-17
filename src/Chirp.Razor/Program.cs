using Chirp.Core.Classes;
using Chirp.Repositories.Interfaces;
using Chirp.Repositories.Repositories;
using Chirp.Services;
using Chirp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString, b => b.MigrationsAssembly("Chirp.Razor")));

builder.Services.AddDefaultIdentity<Author>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddEntityFrameworkStores<ChirpDBContext>();


builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

string? githubClientId = builder.Configuration["GITHUBCLIENTID"];
string? githubClientSecret = builder.Configuration["GITHUBCLIENTSECRET"];
if (string.IsNullOrEmpty(githubClientId) || string.IsNullOrEmpty(githubClientSecret))
{
	throw new Exception("GitHub Client ID and Client Secret must be set in the configuration.");
}

builder.Services.AddAuthentication()
	.AddCookie()
	.AddGitHub(o =>
	{
		o.ClientId = githubClientId; // Need to default to something ??
		o.ClientSecret = githubClientSecret;
		o.Scope.Add("user:email");
		// o.CallbackPath = "/signin-github";
	});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	// From the scope, get an instance of our database context.
	// Through the using keyword, we make sure to dispose it after we are done.
	using var context = scope.ServiceProvider.GetService<ChirpDBContext>();
	if (context == null)
	{
		throw new Exception("Could not get ChirpDBContext from service provider.");
	}

	await context.Database.EnsureCreatedAsync();

	// Execute the migration from code.
	try
	{
		context.Database.Migrate();
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}

	if (app.Environment.IsDevelopment())
		DbInitializer.WipeDatabase(context);

	var authors = DbInitializer.SeedDatabase(context);
	DbInitializer.SetAuthorPasswords(authors, scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
else
{
	app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();

public partial class Program { }