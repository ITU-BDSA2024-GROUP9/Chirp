using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    [FromQuery(Name = "page")]
    public string? QueryPage { get; set; }
    public int PageNumber { get; set; }
    public Range CheepRange {get;set;}
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
        Cheeps = _service.GetCheeps();
    }

    public ActionResult OnGet()
    {
        PageNumber = int.TryParse(QueryPage, out var page) && 0<=page && page<=MathUtil.PageAmount(Cheeps) ? page : 1;
        var startCheep = (PageNumber-1) * 32; //There is no page 0.
        var endCheep = startCheep + 32 >= Cheeps.Count ? Cheeps.Count : startCheep+32;
        CheepRange = new Range(startCheep, endCheep);
        //Ideally, query from sqlite in range, something like List<CheepViewModel> CheepRange = query(startcheep, endcheep)
        
        return Page();
    }
}
