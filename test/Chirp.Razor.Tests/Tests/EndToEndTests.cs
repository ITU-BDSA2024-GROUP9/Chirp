using Chirp.Core.DTO;
using Chirp.Repositories.Repositories;
using Chirp.Services;
using Chirp.Tests.Helpers;
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
		private InMemoryDatabaseHelper _fixture;
		private CheepRepository _cheepRepo;
		private ChirpDBContext _context;
		private CheepService _cheepService;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		[SetUp]
		public async Task Init()
		{
			_fixture = new InMemoryDatabaseHelper();
			_context = _fixture.CreateContext();
			_cheepRepo = new CheepRepository(_context);
			_cheepService = new CheepService(_cheepRepo);
			await ServerHelper.StartServer(); // Starts the server before each test
		}

		[Test]
		public async Task E2ETest()
		{
			//User registers for the first time and accesses the homepage.
			await Page.GotoAsync("http://localhost:5273/");
			await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
			await Page.GetByPlaceholder("name@example.com").ClickAsync();
			await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
			await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
			await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test1!");
			await Page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
			await Page.GetByLabel("Confirm Password").FillAsync("Test1!");
			await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
			await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

			//User logs in.
			await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
			await Page.GetByPlaceholder("name@example.com").ClickAsync();
			await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
			await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
			await Page.GetByPlaceholder("password").FillAsync("Test1!");
			await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

			//User posts a cheep.
			await Page.GetByPlaceholder("Type here!").ClickAsync();
			await Page.GetByPlaceholder("Type here!").FillAsync("Hello, everyone!");
			await Page.GetByPlaceholder("Type here!").ClickAsync();
			await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

			//User goes to their timeline and confirms their cheep is there.
			await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
			await Page.GetByText("test@mail.com Hello, everyone").ClickAsync();

			//The database has registered the user as well as the cheep.
			var result = _cheepService.GetAuthorByEmail("test@mail.com");
			Assert.NotNull(result);

			//Get cheeps from the author.
			var cheeps = _cheepService.GetCheepsFromAuthorByID(result.Id, 1);

			//Assert that the cheep exists in the database
			bool cheepExists = false;
			foreach (CheepDTO cheep in cheeps)
			{
				if (cheep.Text.Equals("Hello, everyone!"))
				{
					cheepExists = true;
				}
			}

			Assert.True(cheepExists);

			//User goes to the public timeline and clicks on a user which isn't them.
			await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
			await Page.GetByRole(AriaRole.Link, new() { Name = "Jacqualine Gilcoine" }).ClickAsync();

			//User follows the user.
			await Page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();

			//User goes to their timeline and confirms the user they followed is there.
			await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
			await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Jacqualine Gilcoine" })).ToBeVisibleAsync();

			//User unfollows the user.
			await Page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" }).ClickAsync();

			//User goes to their timeline and confirms the user they unfollowed is not there.
			await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
			await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Jacqualine Gilcoine" })).Not.ToBeVisibleAsync();
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
