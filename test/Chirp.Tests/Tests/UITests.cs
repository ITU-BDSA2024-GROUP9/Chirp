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
		public async Task TestRegisterAndLogin()
		{
			await Page.GotoAsync("http://localhost:5273/");
			await Helpers.PlaywrightHelper.RegisterAsync(Page, "test", "test@mail.com", "Test1!");
			await Helpers.PlaywrightHelper.LoginAsync(Page, "test", "Test1!");
		}

		[Test]
		public async Task TestAccessUserTimelineAsAnonymousUser()
		{
			await Page.GotoAsync("http://localhost:5273/");
			await Page.Locator("body").ClickAsync();
			await Page.Locator("p").Filter(new() { HasText = "Wendell Ballan At first he" }).GetByRole(AriaRole.Link).ClickAsync();
			await Page.GetByRole(AriaRole.Button, new() { Name = "→" }).ClickAsync();
			await Page.GetByText("Wendell Ballan No great and").ClickAsync();
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
