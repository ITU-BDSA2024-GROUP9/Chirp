using Chirp.Core.DTO;
using Chirp.Repositories.Repositories;
using Chirp.Services;
using Chirp.Tests.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace Chirp.Tests.Tests
{
	[Parallelizable(ParallelScope.Self)]
	[TestFixture]
	public class EndToEndTests : PageTest
	{

		// dissable nullable warnings since the fields are initialized in the setup method
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		private DatabaseHelper _fixture;
		private CheepRepository _cheepRepo;
		private ChirpDBContext _context;
		private CheepService _cheepService;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		[SetUp]
		public async Task Init()
		{
			await ServerHelper.StartServer(); // Starts the server before each test
			Page.SetDefaultTimeout(6000); //Reduce the default timeout
			var path = "../../../../../src/Chirp.Razor/Assets/chirp.db";
			var fullPath = Path.GetFullPath(path);
			var connectionString = $"Data Source={fullPath};";
			_fixture = new DatabaseHelper(connectionString);
			_context = _fixture.CreateContext();
			_cheepRepo = new CheepRepository(_context);
			_cheepService = new CheepService(_cheepRepo);
		}

		[Test]
		[TestCase("test", "test@mail.com", "Test1!", "Hello!")]
		public async Task E2ETest(string username, string email, string password, string cheepMessage)
		{
			await Page.GotoAsync("http://localhost:5273/");

			//User registers
			await PlaywrightHelper.RegisterAsync(Page, username, email, password);
			
			//User posts a cheep.
			await PlaywrightHelper.Cheep(Page, cheepMessage);
			
			//User goes to their timeline and confirms their cheep is there.
			await PlaywrightHelper.AccessOwnTimeline(Page);
			
			Assert.NotNull(Page.GetByText(username + cheepMessage + " Just now", new() {Exact = true}));
			
			//The database has registered the user as an author
			var result = _cheepService.GetAuthorByName(username);
			
			Assert.NotNull(result);

			//Get cheeps from the author.
			var cheeps = _cheepService.GetCheepsFromAuthorByID(result.Id, 1);

			//Assert that the cheep exists in the database
			bool cheepExists = false;
			foreach (CheepDTO cheep in cheeps)
			{
				if (cheep.Text.Equals(cheepMessage))
				{
					cheepExists = true;
				}
			}

			Assert.True(cheepExists);

			//User goes to the public timeline and clicks on a user which isn't them.
			await PlaywrightHelper.AccessPublicTimeline(Page);
			//Just taking a seeded cheep which is always there.
			await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine · aug. 1, 2023 Starbuck now is what we hear the worst. Show" }).GetByRole(AriaRole.Link).ClickAsync();

			//await Page.GetByRole(AriaRole.Link, new() { Name = "Jacqualine Gilcoine" }).ClickAsync();

			//User follows the user.
			await Page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();

			//User goes to their timeline and confirms the user they followed is there.
			await PlaywrightHelper.AccessOwnTimeline(Page);
			await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Jacqualine Gilcoine" }).Nth(0)).ToBeVisibleAsync();

			//User unfollows the user.
			await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine · aug. 1, 2023 Starbuck now is what we hear the worst. Show" }).GetByRole(AriaRole.Link).ClickAsync();
			await Page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" }).ClickAsync();
			
			//User goes to their timeline and confirms the user they unfollowed is not there.
			await PlaywrightHelper.AccessOwnTimeline(Page);
			await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Jacqualine Gilcoine" }).Nth(0)).Not.ToBeVisibleAsync();
		}


		[TearDown]
		public void Cleanup()
		{
			ServerHelper.StopServer(); // Stops the server after each test
			_context.Dispose();
			_fixture.Dispose();
		}

		class TestUtils
		{
			public static string
				UnixTimeStampToDateTimeString(
					double unixTimeStamp) // TODO - Make a util class for this method and use it here and in DBFacade
			{
				// Unix timestamp is seconds past epoch
				var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
				dateTime = dateTime.AddSeconds(unixTimeStamp);
				return dateTime.ToString("MM/dd/yy H:mm:ss");
			}
		}
	}
}
