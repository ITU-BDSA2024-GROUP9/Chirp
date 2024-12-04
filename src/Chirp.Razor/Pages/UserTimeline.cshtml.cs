using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Chirp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : Model
{
    public UserTimelineModel(ICheepService service) : base(service) { }

    public ActionResult OnGet([FromQuery] int page, string author)
    {
        if (page < 1) page = 1;
        base.PaginateCheepsByName(page, author);
        
        return Page();
    }

    public new IActionResult OnPostDeleteCheep(int cheepId, int page = 1)
    {
        return base.OnPostDeleteCheep(cheepId, page);
    }
}