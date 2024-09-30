using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Chirp.Core.Interfaces;
using Chirp.Razor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation(); // Enable runtime compilatio

builder.Services.AddSingleton<ICheepService, CheepService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();  // Ensure detailed error pages in development
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

await app.RunAsync();

public partial class Program { }