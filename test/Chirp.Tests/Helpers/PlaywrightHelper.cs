using Chirp.Core.DTO;
using Microsoft.Playwright;

namespace Chirp.Tests.Helpers;

public static class PlaywrightHelper
{
    public static async Task LoginAsync(IPage page, string userName, string password)
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await page.GetByLabel("Email or Username").FillAsync(userName);
        await page.GetByPlaceholder("password").FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }
    
    public static async Task RegisterAsync(IPage page, string userName, string email, string password)
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await page.GetByPlaceholder("johndoe").FillAsync(userName);
        await page.GetByPlaceholder("name@example.com").FillAsync(email);
        await page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        await page.GetByLabel("Confirm Password").FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

    }

    public static async Task Cheep(IPage page, string cheep)
    {
        await page.GetByPlaceholder("Type here!").FillAsync(cheep);
        await page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
    }

    public static async Task LogoutAsync(IPage page, string userName)
    {
        string roleName = "Logout [" + userName + "]";
        await page.GetByRole(AriaRole.Link, new() { Name = roleName }).ClickAsync();

    }

    public static string? GetAuthorOfMostRecentCheep(IPage page)
    {
        string? cheepText = page.Locator("li").First.TextContentAsync().Result;
        if (cheepText != null)
        {
            return cheepText.Split(" ·")[0].Trim();
        }
        return null;
    }
    
    

    public static async Task AccessOwnTimeline(IPage page)
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
    }
    
    public static async Task AccessPublicTimeline(IPage page)
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
    }
    
    public static async Task TearDown(IPage page, IBrowser browser)
    {
        // Clear cookies
        if (page != null)
        {
            await page.Context.ClearCookiesAsync();
            await page.CloseAsync();

        }

        // Close the page and browser to clean up between tests
        if (browser != null)
        {
            await browser.CloseAsync();
        }
    }

    public static bool Follows(List<AuthorDTO> list)
    {
        bool follows = false;
        foreach(var dto in list){
            if (dto.UserName.Equals("Jacqualine Gilcoine"))
            {
                follows = true;
            }
        }

        return follows;
    }
}