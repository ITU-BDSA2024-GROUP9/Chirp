using Chirp.Tests.Helpers;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Internal;
using helpers = Chirp.Tests.Helpers;


namespace Chirp.Tests.Tests
{
	[Parallelizable(ParallelScope.Self)]
	[TestFixture]
	public class UITests : PageTest
	{
		[SetUp]
		public async Task Init()
		{
			var userName = "test";
			var email = userName + "@mail.com";
			var password = "Test1!";
			await ServerHelper.StartServer(); // Starts the server before each test
			Page.SetDefaultTimeout(12000); //Reduce the default timeout

		}

		[Test]
		[TestCase("test", "test@mail.com", "Test1!")]
		public async Task TestRegisterAndLoginAndLogout(string username, string email, string password)
		{
			await Page.GotoAsync("http://localhost:5273/");
			await PlaywrightHelper.RegisterAsync(Page, username, email, password);
			NUnit.Framework.Assert.True(Page.IsVisibleAsync("text = " + username).Result);
			await PlaywrightHelper.LogoutAsync(Page, username);
			NUnit.Framework.Assert.False(Page.IsVisibleAsync("text = " + username).Result);
		}

		[Test]
		public async Task TestAccessUserTimelineAsAnonymousUser()
		{
			await Page.GotoAsync("http://localhost:5273/");
			string? cheepText = Page.Locator("li").First.TextContentAsync().Result;
			string? authorName = cheepText.Split(" ·")[0].Trim();
			await Page.Locator("li").First.GetByRole(AriaRole.Link).ClickAsync();
			NUnit.Framework.Assert.True(Page.IsVisibleAsync("text = " + authorName + "'s Timeline").Result);
		}

		[Test]
		[TestCase("test", "test@mail.com", "Test1!")]
		public async Task UserRegistersAndAccessesUserTimeline(string username, string email, string password)
		{
			await Page.GotoAsync("http://localhost:5273/");
			await PlaywrightHelper.RegisterAsync(Page, username, email, password);
			var cheepContent = PlaywrightHelper.GetAuthorOfMostRecentCheep(Page);
			await Page.Locator("li").First.GetByRole(AriaRole.Link).ClickAsync(); //Clicks on the author and should therefore be redirected to author's timeline.
			NUnit.Framework.Assert.True(Page.IsVisibleAsync("text = " + cheepContent + "'s Timeline").Result);
		}

		[Test]
		[TestCase("test", "test@mail.com", "Test1!", "Hello!")]

		public async Task UserRegistersAndPostsCheepAndAccessesPrivateTimeline(string username, string email, string password, string cheepMessage)
		{
			await Page.GotoAsync("http://localhost:5273/");
			await PlaywrightHelper.RegisterAsync(Page, username, email, password);
			await PlaywrightHelper.Cheep(Page, cheepMessage);
			await PlaywrightHelper.AccessOwnTimeline(Page);
			NUnit.Framework.Assert.NotNull(Page.GetByText(username + cheepMessage + " Just now", new() {Exact = true}));
		}
		
		[TearDown]
		public void Cleanup()
		{
			ServerHelper.StopServer(); // Stops the server after each test
		}

	}
}
