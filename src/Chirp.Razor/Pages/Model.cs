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
	public string Message { get; set; } = "";

    protected readonly ICheepService _service;

    public int PageNumber { get; set; }

    public int TotalPages { get; set; }
    public Range CheepRange {get;set;}
    public List<CheepDTO>? Cheeps { get; set; }
    public Author? Author { get; set; }
    public Author? userAuthor { get; set; }
    public List<Author>? followedAuthors { get; set; }

    public Model(ICheepService service)
    {
        _service = service;

    }

    //Ideally querying slices instead of taking the whole thing.
    public void PaginateCheeps(int queryPage)
    {
        if (User.Identity != null)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.Identity.Name != null)
                    userAuthor = _service.GetAuthorByName(User.Identity.Name);
                if (userAuthor != null)
                    followedAuthors = _service.getFollowedInCheeps(userAuthor);
            }
        }
        PageNumber = queryPage;
        Cheeps = _service.GetCheeps(queryPage);
        TotalPages = PageAmount(_service.GetCheepCount());
        CheepRange = new Range(0, Cheeps.Count);
    }

    public IActionResult OnPostFollow(string followed)
    {
        if (User.Identity == null) return RedirectToPage();
        if (User.Identity.Name == null) return RedirectToPage(); // these should never happen

        var userToFollow = _service.GetAuthorByName(followed);
        var currentUser = _service.GetAuthorByName(User.Identity.Name);
        if (userToFollow == null || currentUser == null) return RedirectToPage();
        _service.Follow(currentUser, userToFollow);
        Console.WriteLine("Follow might have been a success");
        return RedirectToPage();
    }
    
    public IActionResult OnPostUnfollow(string unfollowed)
    {
        if (User.Identity == null) return RedirectToPage();
        if (User.Identity.Name == null) return RedirectToPage(); // these should never happen

        var userToUnfollow = _service.GetAuthorByName(unfollowed);
        var currentUser = _service.GetAuthorByName(User.Identity.Name);
        if (userToUnfollow == null || currentUser == null) return RedirectToPage();
        _service.Unfollow(currentUser, userToUnfollow);
        Console.WriteLine("Unfollow might have been a success");
        return RedirectToPage();
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
        if (User.Identity != null)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.Identity.Name != null)
                    userAuthor = _service.GetAuthorByName(User.Identity.Name);
                if (userAuthor != null)
                    followedAuthors = _service.getFollowedInCheeps(userAuthor);
            }
        }
        
        PageNumber = queryPage;
        Author = _service.GetAuthorByName(authorName);
        TotalPages = PageAmount(_service.GetCheepByName(authorName));
        Cheeps = _service.GetCheepsFromAuthorByName(authorName, queryPage);
    }

    public void PaginateCheepsByFollowers(int queryPage, string authorName)
    {
        if (User.Identity != null)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.Identity.Name != null)
                    userAuthor = _service.GetAuthorByName(User.Identity.Name);
                if (userAuthor != null)
                    followedAuthors = _service.getFollowedInCheeps(userAuthor);
            }
        }

        PageNumber = queryPage;
        Author = _service.GetAuthorByName(authorName);
        if (followedAuthors == null || userAuthor == null)
        {
            TotalPages = 1; 
            return;
        }
        TotalPages = PageAmount(_service.GetCheepCountByAuthors(followedAuthors, userAuthor.Id));
        Cheeps = _service.GetCheepsFromAuthors(followedAuthors, userAuthor.Id, queryPage);
    }

    private int PageAmount(int totalCheeps)
    {
        int pages = (int) Math.Ceiling(1.0 * totalCheeps / 32);
        return pages <= 0 ? 1 : pages;
    }

    public string isFollower(string userid, string author_userid)
    {
        var user = _service.GetAuthorByID(userid);
        var author = _service.GetAuthorByID(author_userid);
        if (user == null || author == null) return "Follow";
        
        if (_service.IsFollowing(user, author))
        {
            return "Unfollow";
        }
        
        return "Follow";
    }

    public IActionResult OnPostCreateCheep()
    {
        if (!ModelState.IsValid)
        {
            // Repopulate the page data before returning
            PaginateCheeps(1);
            return Page();
        }

        if (User.Identity?.Name == null) return RedirectToPage("/Error");
        var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userid == null) return RedirectToPage("/Error");

        var author = _service.GetAuthorByID(userid);
        if (author == null)return RedirectToPage("/Error");

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