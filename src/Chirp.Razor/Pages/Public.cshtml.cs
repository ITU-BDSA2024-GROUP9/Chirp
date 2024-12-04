using Chirp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Razor.Pages;

public class PublicModel : Model
{
	public PublicModel(ICheepService service) : base(service) { }
	private int infinitePage = 1;

	public ActionResult OnGet([FromQuery] int page)
	{
		if (page < 1) page = 1;
		base.PaginateCheeps(page);
		return Page();
	}

	public PartialViewResult OnGetLoadMoreCheeps()
	{
		infinitePage++;
		base.PaginateCheeps(infinitePage);
		return Partial("_CheepListPartial", (Cheeps, CheepRange, PageNumber, TotalPages, UserAuthor, FollowedAuthors));
	}

	public new IActionResult OnPostDeleteCheep(int cheepId, int page = 1)
	{
		return base.OnPostDeleteCheep(cheepId, page);
	}
}