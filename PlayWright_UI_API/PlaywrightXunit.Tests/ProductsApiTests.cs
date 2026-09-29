namespace PlaywrightXunit.Tests;

using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Playwright;

public class ProductsApiTests : IAsyncLifetime
{
    private IPlaywright? _playwright; // playwright engine
    private IAPIRequestContext? _context; // to trigger api endpoint
    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _context = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
        {
            BaseURL = "http://localhost:5118",
            IgnoreHTTPSErrors = true,
            ExtraHTTPHeaders = new Dictionary<string, string> { { "Accept", "application/json" } }
        });
    }

    [Fact]
    public async Task ProductsApi_ProfileOnLogin()
    {
        Assert.NotNull(_context);

        /* get JWT from login credentials */
        var loginResponse = await _context.PostAsync("/api/auth/login", new()
        {
            DataObject = new { Username = "acdevs", Password = "4444" }
        });

        Assert.Equal(200, loginResponse.Status);

        var token = await loginResponse.JsonAsync();
        Assert.NotNull(token);

        string? jwtToken = token.Value.GetProperty("token").GetString();
        Assert.NotNull(jwtToken);

        /* verify the profile from JWT */
        var profileResponse = await _context.GetAsync("/api/profile", new()
        {
            Headers = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {jwtToken}"}
            }
        });

        Assert.Equal(200, profileResponse.Status);

        var profileData = await profileResponse.JsonAsync();
        Assert.NotNull(profileData);

        string? fullName = profileData.Value.GetProperty("fullName").GetString();
        Assert.NotNull(fullName);
        Assert.Equal("Aman Chandra", fullName);
    }

    public async Task DisposeAsync()
    {
        _playwright?.Dispose();
        if (_context != null) await _context.DisposeAsync();
    }
}