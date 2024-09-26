using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using Chirp.Razor.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Chirp.Razor.Tests;

public class UnitTests
{
    [Fact]
    public void TestGetCheeps()
    {
        // arrange
        ICheepService CheepService = new MockCheepService();

        // act
        List<CheepViewModel> cheeps = CheepService.GetCheeps();

        // assert
        // 
        Assert.NotEmpty(cheeps);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public void TestGetCheepsFromAuthor(string author)
    {
        // arrange
        ICheepService CheepService = new MockCheepService();

        // act
        // TODO - Add insert cheep into to model
        List<CheepViewModel> cheeps = CheepService.GetCheepsFromAuthor(author);

        // assert
        foreach (CheepViewModel cheep in cheeps)
            Assert.Equal(cheep.Author, author);

        // TODO - Rollback changes
    }
}

// This entire class can be found here https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_05/Slides.md#testing-of-web-applications--integration-testing-1
public class TestAPI : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public TestAPI(WebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
    }

    // This test is from https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
    [Theory]
    [InlineData("/")]
    public async Task Get_EndpointsReturnSuccess(string url)
    {
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
    }

    [Fact]
    public async void CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public async void CanSeePrivateTimeline(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }
}

public class IntegrationTests
{

}

public class EndToEndTests
{

}
