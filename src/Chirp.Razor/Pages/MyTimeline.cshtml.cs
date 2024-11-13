
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Razor.Pages;

public class MyTimelineModel : Model
{
    public MyTimelineModel(ICheepService service) : base(service) { }

    public ActionResult OnGet([FromQuery] int page, string author)
    {
        if (page < 1) page = 1;
        base.PaginateCheepsByFollowers(page, author);
        
        return Page();
    }

    public IActionResult OnPostDeleteCheep(int cheepId, int page = 1)
    {
        return base.OnPostDeleteCheep(cheepId, page);
    }
}