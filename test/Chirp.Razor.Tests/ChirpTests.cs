using System.Security.Cryptography.X509Certificates;
using Chirp.Core.Classes;
using Chirp.Core.Helpers;
using Chirp.Core.Interfaces;
using Chirp.Razor.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Chirp.Razor.Tests;

// ref: https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database 
public class TestDatabaseFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestDatabaseFixture()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new ChirpDBContext(options);
        context.Database.EnsureCreated();
        DbInitializer.SeedDatabase(context);
    }

    public ChirpDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(_connection)
            .Options;

        return new ChirpDBContext(options);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}


public class UnitTests
{   
    private readonly TestDatabaseFixture _fixture;

    public UnitTests()
    {
        _fixture = new TestDatabaseFixture();
    }
    
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
    [InlineData("Helge", "Hello, BDSA students!")]
    [InlineData("Adrian", "Hej, velkommen til kurset.")]
    public async Task TestGetCheepsAsync(string authorName, string text)
    {
        // arrange
        using var context = _fixture.CreateContext();
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
    public async Task TestGetCheepsFromAuthorAsync(int authorId)
    {
        // arrange
        using var context = _fixture.CreateContext();
        ICheepService CheepService = new CheepService(new CheepRepository(context));

        // act
        List<CheepDTO> cheeps = CheepService.GetCheepsFromAuthor(authorId);

        // assert
        Assert.NotEmpty(cheeps);
        foreach (CheepDTO cheep in cheeps)
            Assert.Equal(cheep.Author.AuthorId, authorId);
    }

    [Theory]
    [InlineData("Helge")]
    public async Task TestGetCheepsFromAuthorWithName(string name)
    {
        // arrange
        using var context = _fixture.CreateContext();

        ICheepRepository repository = new CheepRepository(context);

        // act 
        var result = repository.GetAuthor(name);

        // assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
    }

    [Theory]
    [InlineData("ropf@itu.dk")]
    public async Task TestGetAuthorWithEmail(string email)
    {
        // Arrange
        using var context = _fixture.CreateContext();
        ICheepService cheepService = new CheepService(new CheepRepository(context));
        
        // Act
        Author author = cheepService.GetAuthorByEmail(email);
        
        // Assert
        Assert.Equal(11, author.AuthorId);
    }

    [Theory]
    [InlineData("Phillip's Mom")]
    public async Task TestCreateAuthor(string newAuthor){
        // arrange
        using var context = _fixture.CreateContext();
        ICheepRepository repository = new CheepRepository(context);
        
        // act)
        repository.CreateAuthor(new Author(){AuthorId = 13, Name = newAuthor, Email = newAuthor + "@gmail.com", Cheeps = new List<Cheep>()});
        var result = repository.GetAuthor(newAuthor);

        // assert
        Assert.NotNull(result);
        Assert.Equal(newAuthor, result.Name);
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

        
    static Int32 SubstringCount(string orig, string find)
    {
        var s2 = orig.Replace(find,"");
        return (orig.Length - s2.Length) / find.Length;
    }

    [Fact]
    public async void PagesLimitedTo32()
    {
        // arrange
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();

        // act
        var content = await response.Content.ReadAsStringAsync();
        // var Substri

        // assert
        Assert.Contains("Chirp!", content);
        Assert.Equal(32, SubstringCount(content, "<p>"));
        
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10)]
    [InlineData("Quintin Sitts", 5)]
    public async void CanSeePrivateTimeline(string author, int authorId)
    {
        var response = await _client.GetAsync($"/{authorId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine")]
    [InlineData("Quintin Sitts")]
    public async void CanSeePrivateTimelineFromName(string author)
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