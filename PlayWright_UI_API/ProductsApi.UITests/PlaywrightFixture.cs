using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsApi.UITests;

public class PlaywrightFixture : IAsyncLifetime
{
    public IPlaywright? Playwright { get; private set; }
    public IAPIRequestContext? Request { get; private set; }

    public string BaseUrl => "http://localhost:5118";

    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Request = await Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = BaseUrl,
            IgnoreHTTPSErrors = true
        });
    }

    public async Task DisposeAsync()
    {
        await Request!.DisposeAsync();
        Playwright!.Dispose();
    }
}
