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
    private const string ConnectionString = @"Data Source=./Assets/chirpunittests.db";

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
                    Author _author1 = new Author() { AuthorId = 1, Name = "John doe", Email = "johndoe@gmail.com", Cheeps = new List<Cheep>()};
                    Author _author2 = new Author() { AuthorId = 2, Name = "Jill doe", Email = "jilldoe@gmail.com", Cheeps = new List<Cheep>()};
                    var time = DateTime.UtcNow;
                    Cheep _c1 = new Cheep() {CheepId = 1, Author = _author1, AuthorId = _author1.AuthorId,  TimeStamp = time, Text = "Chorp"};
                    Cheep _c2 = new Cheep() {CheepId = 2, Author = _author2, AuthorId = _author2.AuthorId,  TimeStamp = time, Text = "Chorpasd"};
                    _author1.Cheeps.Add(_c1);
                    _author2.Cheeps.Add(_c2);

                    context.AddRange(
                        _author1,
                        _author2,
                        _c1,
                        _c2);
                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public ChirpDBContext CreateContext()
        => new ChirpDBContext(
            new DbContextOptionsBuilder<ChirpDBContext>()
                .UseSqlite(ConnectionString)
                .Options);
}

public class UnitTests : IClassFixture<TestDatabaseFixture>
{
    public UnitTests(TestDatabaseFixture fixture)
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
    [InlineData("John doe", "Chorp")]
    [InlineData("Jill doe", "Chorpasd")]
    public void TestGetCheeps(string authorName, string text)
    {
        // arrange
        using var context = Fixture.CreateContext();
        ICheepService CheepService = new CheepService(new CheepRepository(context));

        // act
        var cheeps = CheepService.GetCheeps();

        // assert
        Assert.NotEmpty(cheeps);
        foreach (var cheep in cheeps) {
            if (cheep.Author.Name.Equals(authorName)) {
                if (cheep.Text.Equals(text)){
                    return;
                }
            }
        }

        Assert.Fail("There were no cheeps returned with BOTH the specified author name: " + authorName + " and the text: " + text);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void TestGetCheepsFromAuthor(int authorId)
    {
        // arrange
        using var context = Fixture.CreateContext();
        ICheepService CheepService = new CheepService(new CheepRepository(context));

        // act
        // TODO - Add insert cheep into to model
        List<CheepDTO> cheeps = CheepService.GetCheepsFromAuthor(authorId);

        // assert
        Assert.NotEmpty(cheeps);
        foreach (CheepDTO cheep in cheeps)
            Assert.Equal(cheep.Author.AuthorId, authorId);

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
    [InlineData("Jacqualine Gilcoine", 10)]
    [InlineData("Quintin Sitts", 5)]
    public async void CanSeePrivateTimeline(string author, int authorId)
    {
        var response = await _client.GetAsync($"/{authorId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        //Assert.Fail(content);
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
    [InlineData("Jacqualine Gilcoine", 10)]
    [InlineData("Quintin Sitts", 5)]
    public async void CanSeePrivateTimelineAzure(string author, int authorId)
    {
        var response = await _client.GetAsync($"/{authorId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        //Assert.Fail(content);
        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
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