using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Chirp.Repositories.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.Pages;

public class WipeDBModel : PageModel
{
    private readonly ChirpDBContext _context;
    private readonly IServiceProvider _provider;
    public WipeDBModel(ChirpDBContext context, IServiceProvider provider) {
        _context = context;
        _provider = provider;
    }

    public IActionResult OnPostWipeDB()
    {
        _context.Database.EnsureDeleted();
        _context.Database.Migrate();
        return RedirectToPage("/");
    }



}