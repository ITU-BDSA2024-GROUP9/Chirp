using Microsoft.Playwright;

namespace Chirp.Tests.Helpers;

public static class PlaywrightHelper
{
    public static async Task LoginAsync(IPage page, string userName, string password)
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(userName);
        await page.GetByPlaceholder("password").FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }
    
    public static async Task RegisterAsync(IPage page, string userName, string email, string password)
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        //await page.GetByPlaceholder("johndoe").ClickAsync();
        await page.GetByPlaceholder("johndoe").FillAsync(userName);
        //await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await page.GetByPlaceholder("name@example.com").FillAsync(email);
        //await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        //await Page.GetByLabel("Confirm Password").ClickAsync();
        await page.GetByLabel("Confirm Password").FillAsync(password);
    }

    public static async Task Cheep(IPage page, string cheep)
    {
        await page.GetByPlaceholder("Type here!").FillAsync(cheep);
        await page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
    }

}