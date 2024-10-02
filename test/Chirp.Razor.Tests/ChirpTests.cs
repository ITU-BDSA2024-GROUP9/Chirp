using System.Security.Cryptography.X509Certificates;
using Chirp.Core.Classes;
using Chirp.Core.Helpers;
using Chirp.Core.Interfaces;
using Chirp.Razor.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Chirp.Razor.Tests;

public class TestDatabaseFixture
{
    private const string ConnectionString = @"Data Source=./Assets/chirp.db";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    Author _author = new Author() { AuthorId = 1, Name = "John doe", Email = "johndoe@gmail.com", Cheeps = new List<Cheep>()};

                    context.AddRange(
                        
                        new Cheep { Name = "Blog1", Url = "http://blog1.com" },
                        new Blog { Name = "Blog2", Url = "http://blog2.com" });
                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public static ChirpDBContext CreateContext()
        => new ChirpDBContext(
            new DbContextOptionsBuilder<ChirpDBContext>()
                .UseSqlite(ConnectionString)
                .Options);
}

public class UnitTests : IClassFixture<TestDatabaseFixture>
{
    public BloggingControllerTest(TestDatabaseFixture fixture)
        => Fixture = fixture;

    public TestDatabaseFixture Fixture { get; }
    
    [Fact]
    public void TestCheepInitialization() 
    {
        // Arrange
        var time = DateTime.UtcNow;
        Author _author = new Author() { AuthorId = 1, Name = "John doe", Email = "johndoe@gmail.com", Cheeps = new List<Cheep>()};
        // Act
        Cheep _cheep = new Cheep() {CheepId = 1, Author = _author, AuthorId = _author.AuthorId,  TimeStamp = time, Text = "Chorp"};
        // Assert
        Assert.Equal(_author, _cheep.Author);
        Assert.Equal("Chorp", _cheep.Text);
        Assert.Equal(time, _cheep.TimeStamp);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.", 1690895677)]
    public void TestGetCheeps(string author, string message, double timestamp)
    {
        // arrange
        using var context = Fixture.CreateContext();
        ICheepService CheepService = new CheepService(new CheepRepository(new ChirpDBContext()));
        var cheep = new CheepViewModel(author, message, TestUtils.UnixTimeStampToDateTimeString(timestamp));

        // act
        var cheeps = CheepService.GetCheeps();

        // assert
        Assert.NotEmpty(cheeps);
        Assert.Contains(cheep, cheeps);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine")]
    [InlineData("Quintin Sitts")]
    public void TestGetCheepsFromAuthor(string author)
    {
        // arrange
        ICheepService CheepService = new CheepService();

        // act
        // TODO - Add insert cheep into to model
        List<CheepViewModel> cheeps = CheepService.GetCheepsFromAuthor(author);

        // assert
        foreach (CheepViewModel cheep in cheeps)
            Assert.Equal(cheep.Author, author);

        // TODO - Rollback changes
    }
}

// This class was based on https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_05/Slides.md#testing-of-web-applications--integration-testing-1
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public IntegrationTests(WebApplicationFactory<Program> fixture)
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
    [InlineData("Jacqualine Gilcoine")]
    [InlineData("Quintin Sitts")]
    public async void CanSeePrivateTimeline(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }
}

public class EndToEndTests
{
    private readonly HttpClient _client;

    public EndToEndTests()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://bdsagroup09chirpremotedb.azurewebsites.net/");
    }

    // This test is from https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
    [Theory]
    [InlineData("/")]
    public async Task Get_EndpointsReturnSuccessAzure(string url)
    {
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
    }

    // This test can be found here https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_05/Slides.md#testing-of-web-applications--integration-testing-1
    [Fact]
    public async void CanSeePublicTimelineAzure()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }

    // This was based on https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_05/Slides.md#testing-of-web-applications--integration-testing-1
    [Theory]
    [InlineData("Jacqualine Gilcoine")]
    [InlineData("Quintin Sitts")]
    public async void CanSeePrivateTimelineAzure(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }
}

class MockEmptyDB : ICheepService
{
    public List<CheepViewModel> GetCheeps()
    {
        return new List<CheepViewModel>();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return new List<CheepViewModel>();
    }
}

class TestUtils
{
    public static string UnixTimeStampToDateTimeString(double unixTimeStamp) // TODO - Make a util class for this method and use it here and in DBFacade
    {
        // Unix timestamp is seconds past epoch
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}