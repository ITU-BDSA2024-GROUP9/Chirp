using Chirp.Tests.Helpers;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Internal;
using helpers = Chirp.Tests.Helpers;


namespace Chirp.Tests.Tests
{
	[NonParallelizable]
	[TestFixture]
	public class UITests : PageTest
	{
		
		private IPage _page;
		private IBrowser _browser;
		[SetUp]
		public async Task Init()
		{ 
			_browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
			_page = await _browser.NewPageAsync();
			_page.SetDefaultTimeout(6000); //Reduce the default timeout
		}

		[Test]
		[NonParallelizable]
		[TestCase("uitester1", "uitester1@mail.com", "Test1!")]
		public async Task TestRegisterAndLoginAndLogout(string username, string email, string password)
		{
			await _page.GotoAsync("http://localhost:5273/");
			await PlaywrightHelper.RegisterAsync(_page, username, email, password);
			NUnit.Framework.Assert.True(_page.IsVisibleAsync("text = " + username).Result);
			await PlaywrightHelper.LogoutAsync(_page, username);
			NUnit.Framework.Assert.False(_page.IsVisibleAsync("text = " + username).Result);
		}
		[Test]
		[NonParallelizable]
		public async Task TestAccessUserTimelineAsAnonymousUser()
		{
			await _page.GotoAsync("http://localhost:5273/");
			string? cheepText = _page.Locator("li").First.TextContentAsync().Result;
			string? authorName = cheepText.Split(" ·")[0].Trim();
			await _page.Locator("li").First.GetByRole(AriaRole.Link).ClickAsync();
			NUnit.Framework.Assert.True(_page.IsVisibleAsync("text = " + authorName + "'s Timeline").Result);
		}
		[Test]
		[NonParallelizable]
		[TestCase("uitester2", "uitester2@mail.com", "Test1!")]
		public async Task UserRegistersAndAccessesUserTimeline(string username, string email, string password)
		{
			await _page.GotoAsync("http://localhost:5273/");
			await PlaywrightHelper.RegisterAsync(_page, username, email, password);
			var cheepContent = PlaywrightHelper.GetAuthorOfMostRecentCheep(_page);
			await _page.Locator("li").First.GetByRole(AriaRole.Link).ClickAsync(); //Clicks on the author and should therefore be redirected to author's timeline.
			NUnit.Framework.Assert.True(_page.IsVisibleAsync("text = " + cheepContent + "'s Timeline").Result);
		}
		[Test]
		[NonParallelizable]
		[TestCase("uitester3", "uitester3@mail.com", "Test1!", "Hello!")]
		public async Task UserRegistersAndPostsCheepAndAccessesPrivateTimeline(string username, string email, string password, string cheepMessage)
		{
			await _page.GotoAsync("http://localhost:5273/");
			await PlaywrightHelper.RegisterAsync(_page, username, email, password);
			await PlaywrightHelper.Cheep(_page, cheepMessage);
			await PlaywrightHelper.AccessOwnTimeline(_page);
			NUnit.Framework.Assert.NotNull(_page.GetByText(username + cheepMessage + " Just now", new() {Exact = true}));
		}
		[TearDown]
		public async Task Cleanup()
		{
			await PlaywrightHelper.TearDown(_page, _browser);
		}
		
		[OneTimeTearDown]
		public void StopServer()
		{
			ServerHelper.StopServer(); //Stops the server once all tests are done.
		}

	}
}