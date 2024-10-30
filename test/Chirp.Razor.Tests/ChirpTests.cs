using System.ComponentModel.DataAnnotations;
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
    private ChirpDBContext context;

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


public class UnitTests : IDisposable
{   
    private readonly TestDatabaseFixture _fixture;
    private readonly CheepRepository _cheepRepo;
    private readonly ChirpDBContext _context;
    
    public UnitTests()
    {
        _fixture = new TestDatabaseFixture();
        _context = _fixture.CreateContext();
        _cheepRepo = new CheepRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _fixture.Dispose();
    }
    
    [Fact]
    public void TestCheepInitialization() 
    {
    }

    [Theory]
    [InlineData("Helge", "Hello, BDSA students!")]
    [InlineData("Adrian", "Hej, velkommen til kurset.")]
    public void TestGetCheeps(string authorName, string text)
    {
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void TestGetCheepsFromAuthor(int authorId)
    {

    }

    [Theory]
    [InlineData("Helge")]
    public void TestGetCheepsFromAuthorWithName(string name)
    {

    }

    [Theory]
    [InlineData("ropf@itu.dk")]
    public void TestGetAuthorWithEmail(string email)
    {

    }

    [Theory]
    [InlineData("13", "John Doe", "johndoe@yahoo.com")]
    public void TestCreateAuthor(string id, string newAuthor, string email){
        // arrange
        var author = new Author(){Cheeps = new List<Cheep>()};
        author.Id = id;
        author.UserName = newAuthor;
        author.Email = email;
        
        // act
        _cheepRepo.CreateAuthor(author);

        // assert
        var result = _cheepRepo.GetAuthor(id);

        Assert.NotNull(result);
        Assert.Equal(newAuthor, result.UserName);
    }

    [Theory]
    [InlineData(
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.")]
    public void TestCheepCannotBeLongerThan160Characters(string text)
    {

    }
}

// This class was based on https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_05/Slides.md#testing-of-web-applications--integration-testing-1
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public IntegrationTests(WebApplicationFactory<Program> fixture)
    {
        _fixture = fixture.WithWebHostBuilder(Builder =>
        {
            Builder.UseUrls("http://localhost:5273");
        });
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