using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcWithPlaywrightDemo.Tests
{
    public class ItemsUITests
    {
        private const string BaseUrl = "https://localhost:7102";

        [Fact]
        public async Task AddItem_ShouldAppearInListWithIdNamePrice()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            await page.GotoAsync($"{BaseUrl}/Items/Create");

            await page.FillAsync("input[name='Name']", "Laptop");
            await page.FillAsync("input[name='Price']", "90000");

            await page.ClickAsync("button[type='submit']");

            // Wait for the item entry to appear
            var listItem = await page.WaitForSelectorAsync(".item-entry");

            var text = await listItem!.InnerTextAsync();
            Assert.Contains("Laptop - 90000", text);
        }

        [Fact]
        public async Task EmptyItem_ShouldShowError()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            await page.GotoAsync($"{BaseUrl}/Items/Create");

            await page.ClickAsync("button[type='submit']");

            // Wait for validation summary
            await page.WaitForSelectorAsync(".validation-summary-errors");

            var errorText = await page.InnerTextAsync(".validation-summary-errors");
            Assert.Contains("Name and Price are required", errorText);
        }

    }
}

