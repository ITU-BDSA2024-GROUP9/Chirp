using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class MyTimelineModel : Model
{
    public MyTimelineModel(ICheepService service) : base(service) { }

    public ActionResult OnGet([FromQuery] int page, string author)
    {
        // Default to the current user's name if `author` is not provided
        if (string.IsNullOrEmpty(author))
        {
            author = User.Identity?.Name;
            Console.WriteLine("Defaulted author to User.Identity.Name: " + author);
        }
        
        if (!string.IsNullOrEmpty(author))
            base.PaginateCheepsByName(page, author);

        return Page();
    }

    public IActionResult OnPostDeleteCheep(int cheepId, int page = 1)
    {
        return base.OnPostDeleteCheep(cheepId, page);
    }
}