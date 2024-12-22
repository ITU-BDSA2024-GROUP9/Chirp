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
			await PlaywrightHelper.LoginAsync(Page, "test", "Test1!");
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
			Console.WriteLine("authname" + authorName);
			await Page.Locator("li").First.GetByRole(AriaRole.Link).ClickAsync();
			NUnit.Framework.Assert.True(Page.IsVisibleAsync("text = " + authorName + "'s Timeline").Result);
		}

		[Test]
		public async Task UserRegistersAndAccessesUserTimeline()
		{
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
			await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
			await Page.GetByPlaceholder("name@example.com").ClickAsync();
			await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
			await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
			await Page.GetByPlaceholder("password").FillAsync("Test1!");
			await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
			await Page.Locator("p").Filter(new() { HasText = "Jacqualine Gilcoine And then" }).GetByRole(AriaRole.Link).ClickAsync();
			await Page.GetByText("Jacqualine Gilcoine In various enchanted attitudes, like the Sperm Whale. — 01/").ClickAsync();
		}

		[Test]
		public async Task UserRegistersAndPostsCheepAndAccessesPrivateTimeline()
		{
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
			await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
			await Page.GetByPlaceholder("name@example.com").ClickAsync();
			await Page.GetByPlaceholder("name@example.com").FillAsync("test");
			await Page.GetByPlaceholder("name@example.com").PressAsync("ArrowLeft");
			await Page.GetByPlaceholder("name@example.com").PressAsync("ArrowRight");
			await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
			await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
			await Page.GetByPlaceholder("password").FillAsync("Test1!");
			await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
			await Page.GetByPlaceholder("Type here!").ClickAsync();
			await Page.GetByPlaceholder("Type here!").FillAsync("Hi everyone!");
			await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
			await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
			await Page.GetByText("test@mail.com Hi everyone! —").ClickAsync();
		}



		[TearDown]
		public void Cleanup()
		{
			ServerHelper.StopServer(); // Stops the server after each test
		}

	}
}
