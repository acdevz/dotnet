using Microsoft.Playwright;
using Xunit;
using System.Threading.Tasks;

public class DemoTest
{
    [Fact]
    public async Task Homepage_ShouldHaveTitle()
    {
        // 1. Start Playwright engine
        using var playwright = await Playwright.CreateAsync();

        // 2. Launch a Chromium browser (headless by default)
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        // 3. Create a new browser context (isolated session)
        var context = await browser.NewContextAsync();

        // 4. Open a new page (tab) inside the context
        var page = await context.NewPageAsync();

        // 5. Navigate to the target URL
        await page.GotoAsync("https://google.com");
        await page.FillAsync("textarea[name='q']", "Playwright");
        await page.ClickAsync("input[type='submit']");

        // 6. Get the page title and assert it contains expected text
        var title = await page.TitleAsync();
        Assert.Contains("Playwright", title);
    }
}
