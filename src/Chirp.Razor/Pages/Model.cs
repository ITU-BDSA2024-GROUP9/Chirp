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
	public string Message { get; set; }

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
        PageNumber = queryPage;
        Cheeps = _service.GetCheeps(queryPage);
        TotalPages = PageAmount(_service.GetCheepCount());
        CheepRange = new Range(0, Cheeps.Count);
    }

    public void FollowAuthor()
    {
        
    }

    public void PaginateCheeps(int queryPage, string authorID)
    {
        PageNumber = queryPage;
        Author = _service.GetAuthorByID(authorID);
        Cheeps = _service.GetCheepsFromAuthorByID(authorID, queryPage);
        TotalPages = PageAmount(_service.GetCheepCountByID(authorID));
    }

    public void PaginateCheepsByName(int queryPage, string authorName)
    {
        PageNumber = queryPage;
        Author = _service.GetAuthorByName(authorName);
        TotalPages = PageAmount(_service.GetCheepByName(authorName));
        Cheeps = _service.GetCheepsFromAuthorByName(authorName, queryPage);
    }

    public string getAuthorID(string authorName)
    {
        return _service.GetAuthorByName(authorName).Id;
    }

    private int PageAmount(int totalCheeps)
    {
        int pages = (int) Math.Ceiling(1.0 * totalCheeps / 32);
        return pages <= 0 ? 1 : pages;
    }

    public string isFollower(string userid, string author_userid)
    {
        
        if (_service.IsFollowing(_service.GetAuthorByID(userid), _service.GetAuthorByID(author_userid)))
        {
            return "Unfollow";
        }
        
        return "Follow";
    }

    public IActionResult OnPostCreateCheep()
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

        var author = _service.GetAuthorByID(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (author == null)
        {
            Console.WriteLine("Died at Author: " + User.Identity.Name);
            return RedirectToPage("/Error");
        }
        var cheep = new CheepDTO
        {
            CheepId = 0,
            Text = Message,
            Author = author,
            TimeStamp = DateTimeOffset.Now.DateTime
        };

        _service.CreateCheep(cheep);

        return RedirectToPage();
    }

    protected IActionResult OnPostDeleteCheep(int cheepId, int page = 1)
    {
        ModelState.Remove("Message");
        if (!ModelState.IsValid)
        {
            PaginateCheeps(page);
            return Page();
        }

        if (User.Identity?.Name == null)
        {
            return RedirectToPage("/Error");
        }

        var cheep = _service.GetCheepByID(cheepId);
        if (cheep == null || cheep.Author.UserName != User.Identity.Name)
        {
            return RedirectToPage("/Error");
        }

        _service.DeleteCheep(cheepId);

        return RedirectToPage();
    }
}