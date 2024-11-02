using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Chirp.Razor.Pages;
public class Model : PageModel
{
	[BindProperty]
	[Required]
	[StringLength(160, ErrorMessage = "Maximum length is {1}")]
	[Display(Name = "Message Text")]
	public required string Message { get; set; }

    private readonly ICheepService _service;

    public int PageNumber { get; set; }

    public int TotalPages { get; set; }
    public Range CheepRange {get;set;}
    public required List<CheepDTO> Cheeps { get; set; }
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

    public void PaginateCheeps(int queryPage, string authorID)
    {
        Cheeps = _service.GetCheepsFromAuthorByID(authorID);
        Paginate(queryPage);
        Author = _service.GetAuthorByID(authorID);
    }

    public void PaginateCheepsByName(int queryPage, string authorName)
    {
        Cheeps = _service.GetCheepsFromAuthorByName(authorName);
        Paginate(queryPage);
        Author = _service.GetAuthorByName(authorName);
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

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            Console.WriteLine("Died at ModelState");
            // Repopulate the page data before returning
            PaginateCheeps(1);
            return Page();
        }

        if (User.Identity?.Name == null)
        {
            Console.WriteLine("Died at Name: " + User.Identity);
            return RedirectToPage("/Error");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            Console.WriteLine("User ID not found.");
            return RedirectToPage("/Error");
        }
        var author = _service.GetAuthorByID(userId);
        if (author == null)
        {
            Console.WriteLine("Died at Author: " + User.Identity.Name);
            return RedirectToPage("/Error");
        }
        var cheep = new CheepDTO
        {
            Text = Message,
            Author = author,
            TimeStamp = DateTimeOffset.Now.DateTime
        };

        _service.CreateCheep(cheep);

        return RedirectToPage();
    }
}