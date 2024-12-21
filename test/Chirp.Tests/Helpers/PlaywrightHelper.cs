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
}