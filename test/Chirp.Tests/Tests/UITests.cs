using Chirp.Tests.Helpers;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
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
			await ServerHelper.StartServer(); // Starts the server before each test
			Page.SetDefaultTimeout(6000); //Reduce the default timeout

		}

		[Test]
		public async Task TestRegisterAndLoginAndLogout()
		{
			await Page.GotoAsync("http://localhost:5273/");
			var userName = "test";
			var email = userName + "@mail.com";
			var password = "Test1!";
			await PlaywrightHelper.RegisterAsync(Page, userName, email, password);
			await PlaywrightHelper.LoginAsync(Page, userName, password);
			NUnit.Framework.Assert.True(Page.IsVisibleAsync("text = " + userName).Result);
			await PlaywrightHelper.LogoutAsync(Page, userName);
			NUnit.Framework.Assert.False(Page.IsVisibleAsync("text = " + userName).Result);
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
		public async Task UserRegistersAndAccessesUserTimeline()
		{
			await Page.GotoAsync("http://localhost:5273/");
			var userName = "test";
			var email = userName + "@mail.com";
			var password = "Test1!";
			await PlaywrightHelper.RegisterAsync(Page, userName, email, password);
			await PlaywrightHelper.LoginAsync(Page, userName, password);
			var cheepContent = PlaywrightHelper.GetAuthorOfMostRecentCheep(Page);
			await Page.Locator("li").First.GetByRole(AriaRole.Link).ClickAsync(); //Clicks on the author and should therefore be redirected to author's timeline.
			NUnit.Framework.Assert.True(Page.IsVisibleAsync("text = " + cheepContent + "'s Timeline").Result);
		}

		[Test]
		public async Task UserRegistersAndPostsCheepAndAccessesPrivateTimeline()
		{
			await Page.GotoAsync("http://localhost:5273/");
			var userName = "test";
			var email = userName + "@mail.com";
			var password = "Test1!";
			var cheepMessage = "Hello";
			await PlaywrightHelper.RegisterAsync(Page, userName, email, password);
			await PlaywrightHelper.LoginAsync(Page, userName, password);
			await PlaywrightHelper.Cheep(Page, cheepMessage);
			await PlaywrightHelper.AccessOwnTimeline(Page);
			NUnit.Framework.Assert.NotNull(Page.GetByText(userName + cheepMessage + " Just now", new() {Exact = true}));
		}
		
		[TearDown]
		public void Cleanup()
		{
			ServerHelper.StopServer(); // Stops the server after each test
		}

	}
}
