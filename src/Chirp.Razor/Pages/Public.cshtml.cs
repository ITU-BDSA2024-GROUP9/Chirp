using System.Transactions;
using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : Model
{
    public PublicModel(ICheepService service) : base(service) { }

    public ActionResult OnGet([FromQuery] int page)
    {
        if (page < 1) page = 1;
        base.PaginateCheeps(page);
        return Page();
    }

    public new IActionResult OnPostDeleteCheep(int cheepId, int page = 1)
    {
        return base.OnPostDeleteCheep(cheepId, page);
    }
}