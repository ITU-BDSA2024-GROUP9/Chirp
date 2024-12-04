using Chirp.Core.Helpers;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Chirp.Razor.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class UITests : PageTest
{
    [SetUp]
    public async Task Init()
    {
        await MyEndToEndUtil.StartServer(); // Starts the server before each test
    }

    [Test]
    public async Task TestRegisterAndLogin()
    {
        await Page.GotoAsync("http://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("johndoe").ClickAsync();
        await Page.GetByPlaceholder("johndoe").FillAsync("testuser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test1!");
        await Page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
        await Page.GetByLabel("Confirm Password").FillAsync("Test1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        //We should be automatically logged in.
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
    }
    
    [Test]
    public async Task TestAccessUserTimelineAsAnonymousUser()
    {
        await Page.GotoAsync("http://localhost:5273/");
        await Page.Locator("body").ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine · Aug 1, 2023 Starbuck now is what we hear the worst. Hide" }).GetByRole(AriaRole.Link).ClickAsync();
        await Page.GetByText("Starbuck now is what we hear").ClickAsync();
    }   

    [Test]
    public async Task UserRegistersAndAccessesUserTimeline()
    {
        //Registering
        await Page.GotoAsync("http://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("johndoe").ClickAsync();
        await Page.GetByPlaceholder("johndoe").FillAsync("testuser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test1!");
        await Page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
        await Page.GetByLabel("Confirm Password").FillAsync("Test1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        
        //Access Timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
    }

    [Test]
    public async Task UserRegistersAndPostsCheepAndAccessesPrivateTimeline()
    {
        //Registering
        await Page.GotoAsync("http://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("johndoe").ClickAsync();
        await Page.GetByPlaceholder("johndoe").FillAsync("testuser");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test1!");
        await Page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
        await Page.GetByLabel("Confirm Password").FillAsync("Test1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
     
        //Write cheep
        await Page.GetByPlaceholder("Type here!").ClickAsync();
        await Page.GetByPlaceholder("Type here!").FillAsync("Hello all!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Page.GetByText("Hello all!").ClickAsync();
        
        //See cheep on own timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
        await Page.GetByText("Hello all!").ClickAsync();
    }



    [TearDown]
    public void Cleanup()
    {
        MyEndToEndUtil.StopServer(); // Stops the server after each test
    }

}