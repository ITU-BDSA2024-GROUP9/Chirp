using System.Transactions;
using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : Model
{
    [FromQuery(Name = "page")]
    public string? QueryPage { get; set; }

    public PublicModel(ICheepService service) : base(service) { }

    public ActionResult OnGet()
    {
        base.PaginateCheeps(QueryPage);
        return Page();
    }
}
