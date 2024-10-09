using Chirp.Core.Interfaces;
using Chirp.Core.Helpers;
using Chirp.Razor.Services;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Classes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString, b => b.MigrationsAssembly("Chirp.Razor")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // From the scope, get an instance of our database context.
    // Through the using keyword, we make sure to dispose it after we are done.
    using var context = scope.ServiceProvider.GetService<ChirpDBContext>();

    // Execute the migration from code.
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();

public partial class Program { }