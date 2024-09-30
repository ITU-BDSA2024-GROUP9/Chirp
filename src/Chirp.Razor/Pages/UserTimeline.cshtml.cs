using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : Model
{
    public UserTimelineModel(ICheepService service) : base(service) { }



    public ActionResult OnGet([FromQuery] int page, string author)
    {
        if (page < 1) page = 1;
        base.PaginateCheeps(page, author);
        return Page();
    }
}