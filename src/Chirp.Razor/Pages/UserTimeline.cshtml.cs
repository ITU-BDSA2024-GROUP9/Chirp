using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : Model
{
    [FromQuery(Name = "page")]
    public string? QueryPage { get; set; }

    public UserTimelineModel(ICheepService service) : base(service) { }



    public ActionResult OnGet(string author)
    {
        base.PaginateCheeps(QueryPage, author);
        return Page();
    }
}
