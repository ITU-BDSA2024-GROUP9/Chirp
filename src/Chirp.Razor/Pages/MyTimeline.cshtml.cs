using Chirp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Razor.Pages;

public class MyTimelineModel : Model
{
	public MyTimelineModel(ICheepService service) : base(service) { }

	public ActionResult OnGet([FromQuery] int page, string? author)
	{
		if (page < 1) page = 1;
		// Default to the current user's name if `author` is not provided
		if (string.IsNullOrEmpty(author))
		{
			author = User.Identity?.Name;
		}

		if (!string.IsNullOrEmpty(author))
			PaginateCheepsByFollowers(page, author);

		return Page();
	}

	public new IActionResult OnPostDeleteCheep(int cheepId, int page = 1)
	{
		return base.OnPostDeleteCheep(cheepId, page);
	}
}