using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Chirp.Razor.Pages;
public abstract class Model : PageModel
{
    private readonly ICheepService _service;

    public int PageNumber { get; set; }

    public int TotalPages { get; set; }
    public Range CheepRange {get;set;}
    public List<CheepDTO> Cheeps { get; set; }
    public Author? Author { get; set; }

    public Model(ICheepService service)
    {
        _service = service;
    }
    //Ideally querying slices instead of taking the whole thing.
    public void PaginateCheeps(int queryPage)
    {
        Cheeps = _service.GetCheeps();
        Paginate(queryPage);
    }

    public void PaginateCheeps(int queryPage, int author)
    {
        Cheeps = _service.GetCheepsFromAuthor(author);
        Paginate(queryPage);
        Author = _service.GetAuthor(author);
    }

    private void Paginate(int queryPage)
    {
        PageNumber = queryPage;
        TotalPages = PageAmount();
        var startCheep = (PageNumber-1) * 32; //There is no page 0.
        var endCheep = startCheep + 32 >= Cheeps.Count ? Cheeps.Count : startCheep+32;
        CheepRange = new Range(startCheep, endCheep);
    }

    private int ParsePageNumber(string? queryPage)
    {
        if (int.TryParse(queryPage, out var page))
        {
            if (0 <= page && page <= TotalPages)
            {
                return page;
            }
        }
        return 1;
    }
    
    private int PageAmount()
    {
        return (int) Math.Ceiling(1.0 * Cheeps.Count / 32);
    }
}