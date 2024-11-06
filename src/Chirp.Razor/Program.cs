using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Helpers;
using Chirp.Razor.Services;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Classes;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNet.Security.OAuth.GitHub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString, b => b.MigrationsAssembly("Chirp.Razor")));

builder.Services.AddDefaultIdentity<Author>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ChirpDBContext>();


builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication()
    .AddCookie()
    .AddGitHub(o =>
    {
        o.ClientId = builder.Configuration["GITHUBCLIENTID"]; // Need to default to something ??
        o.ClientSecret = builder.Configuration["GITHUBCLIENTSECRET"];
        // o.CallbackPath = "/signin-github";
    });


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // From the scope, get an instance of our database context.
    // Through the using keyword, we make sure to dispose it after we are done.
    using var context = scope.ServiceProvider.GetService<ChirpDBContext>();

    // Execute the migration from code.
    try {
        context.Database.Migrate();
    } catch (Exception ex) {
        Console.WriteLine(ex.Message);
    }
    DbInitializer.SeedDatabase(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else {
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
// app.UseSession();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();

public partial class Program { }