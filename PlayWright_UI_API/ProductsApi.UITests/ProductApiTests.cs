using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductsApi.UITests;

public class ProductApiTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public ProductApiTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task LoginAndGetProducts_ShouldReturnLaptop()
    {
        // Login with valid credentials
        var loginResponse = await _fixture.Request!.PostAsync("/api/auth/login", new APIRequestContextOptions
        {
            DataObject = new { Username = "acdevs", Password = "4444" }
        });

        Assert.Equal(200, loginResponse.Status);

        var loginJson = await loginResponse.JsonAsync();
        var token = loginJson!.Value.GetProperty("token").GetString();
        Assert.False(string.IsNullOrEmpty(token), "Token not found in login response");

        var productsResponse = await _fixture.Request.GetAsync("/api/products", new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {token}"
            }
        });

        Assert.Equal(200, productsResponse.Status);
        var productsText = await productsResponse.TextAsync();
        Assert.Contains("Laptop", productsText);
    }

    [Fact]
    public async Task LoginWithInvalidCredentials_ShouldFail()
    {
        // Login with wrong password
        var loginResponse = await _fixture.Request!.PostAsync("/api/auth/login", new APIRequestContextOptions
        {
            DataObject = new { Username = "acdevs", Password = "wrongpass" }
        });

        Assert.Equal(401, loginResponse.Status);
    }
}
