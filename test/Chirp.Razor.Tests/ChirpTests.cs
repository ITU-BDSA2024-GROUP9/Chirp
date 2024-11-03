using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Chirp.Core.Classes;
using Chirp.Core.Helpers;
using Chirp.Core.Interfaces;
using Chirp.Razor.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

[Parallelizable(ParallelScrope.Self)]
[TestFixture]
public class Tests : PageTest
{
    [Test]
    public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingToTheIntroPage()
    {
        await Page.GotoAsync("http://localhost:5273");
        // Expect a title "to contain" a substring
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
        
        // Create a locator
        var getStarted = Page.GetByRole(AriaRole.Link, new() { Name = "Get Started" });
        
        // Expect an attribute "to be strictly equal" to the value.
        await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");
        
        // Click the get started link
        await getStarted.ClickAsync();
        // Expects the url to contain intro
        await Expect(Page).ToHaveURLAsync(new Regex((".*intro")));

    }
}

public class UnitTests : IDisposable
{   
    private readonly TestDatabaseFixture _fixture;
    private readonly CheepRepository _cheepRepo;
    private readonly ChirpDBContext _context;
    private readonly CheepService _cheepService;
    
    public UnitTests()
    {
        _fixture = new TestDatabaseFixture();
        _context = _fixture.CreateContext();
        _cheepRepo = new CheepRepository(_context);
        _cheepService = new CheepService(_cheepRepo);
    }

    public void Dispose()
    {
        _context.Dispose();
        _fixture.Dispose();
    }

    [Theory]
    [InlineData("Hej med dig smukke", "11")]
    public void TestCreateCheeps(string text, string authorID)
    {
        // Arrange
        var cheep = new CheepDTO()
        {
            Text = text,
            TimeStamp = DateTime.Now,
            Author = _cheepService.GetAuthorByID(authorID)
        };
        
        // Act
        _cheepService.CreateCheep(cheep);
        var result = _cheepService.GetCheepsFromAuthorByID(authorID);
        
        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(result.Last().Text, text);
    }

    [Theory]
    [InlineData("11")] // Helge's ID
    public void RetrieveAllDataRelatedToAuthor(string ID)
    {
        // Arrange
        
        // Act
        var result = _cheepService.GetAuthorByID(ID);
        // Assert
        Assert.Equal("11", result.Id);
        Assert.Equal("ropf@itu.dk", result.Email);
        Assert.Equal("Helge", result.UserName);
        Assert.NotEmpty(result.Cheeps);
    }
    
    [Theory]
    [InlineData("Hej med dig, det her er en test")]
    public void TestCreateCheepsWithNewAuthor(string text)
    {
        // Arrange
        var author = new Author()
        {
            UserName = "testy",
            Email = "testyeeawea",
            Cheeps = new List<Cheep>()
        };
        
        var cheep = new CheepDTO()
        {
            Text = text,
            TimeStamp = DateTime.Now,
            Author = author
        };
        
        // Act
        _cheepRepo.CreateAuthor(author);
        _cheepService.CreateCheep(cheep);
        var result = _cheepService.GetCheepsFromAuthorByName(author.UserName);
       
        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(result[0].Text, text);
    }
    
    [Theory]
    [InlineData("11")]
    [InlineData("12")]
    public void TestGetCheeps(string id)
    {
        // Arrange
        
        
        // Act
        Author author = _cheepRepo.GetAuthorByID(id);
        List<CheepDTO> authorCheeps = _cheepRepo.ReadCheepsByID(author.Id);
        // Assert
        bool success = true;

        foreach (CheepDTO cheep in authorCheeps)
        {
            if (cheep.Author.Id != author.Id)
            {
                success = false;
            }
        }

        Assert.True(success);

    }

    [Fact]
    public void TestThatACheepCanBeUpdated()
    {
        // Arrange
        var timeStamp = DateTime.Now;
        var originalText = "This is the original test";
        var updatedText = "This is the updated test";
        
        var author = new Author()
        {
            UserName = "testName",
            Email = "testName@test.dk",
            Cheeps = new List<Cheep>()
        };
        var originalCheep = new CheepDTO()
        {
            Text = originalText,
            TimeStamp = timeStamp,
            Author = author
        };
        var updatedCheep = new CheepDTO()
        {
            Text = updatedText,
            TimeStamp = timeStamp,
            Author = author
        };
        // Act
        _cheepRepo.CreateAuthor(author);
        var cheepID = _cheepService.CreateCheep(originalCheep);
        var originalResult = _cheepService.GetCheepsFromAuthorByID(author.Id)[0].Text;
        _cheepService.UpdateCheep(updatedCheep, cheepID);
        var updatedResult = _cheepService.GetCheepsFromAuthorByID(author.Id)[0].Text;
        // Assert
        Assert.Equal(originalText, originalResult);
        Assert.Equal(updatedText, updatedResult);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public void TestGetCheepsFromAuthorWithID(string authorId)
    {
        // Arrange
        
        // Act
        var result = _cheepService.GetCheepsFromAuthorByID(authorId);
        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(result[0].Author.Id, authorId);
    }

    [Theory]
    [InlineData("Helge")]
    public void TestGetCheepsFromAuthorWithName(string name)
    {
        // Arrange
        
        // Act
        var result = _cheepService.GetCheepsFromAuthorByName(name);
        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(result[0].Author.UserName, name);
    }

    [Theory]
    [InlineData("11", "Helge")]
    public void TestGetAuthorWithId(string id, string userName)
    {
        //Act
        var result = _cheepService.GetAuthorByID(id);
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(userName, result.UserName);
        Assert.Equal(id, result.Id);
    }
    
    [Theory]
    [InlineData("11", "Helge")]
    public void TestGetAuthorWithName(string id, string userName)
    {
        //Act
        var result = _cheepService.GetAuthorByName(userName);
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }
    
    [Theory]
    [InlineData("11", "ropf@itu.dk")]
    public void TestGetAuthorWithEmail(string id, string email)
    {
        //Act
        var result = _cheepService.GetAuthorByEmail(email);
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
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
        var result = _cheepService.GetAuthorByID(id);

        Assert.NotNull(result);
        Assert.Equal(newAuthor, result.UserName);
    }
    [Theory]
    [InlineData("Test", "Test@test.dk")]
    public void TestYouCannotCreateACheepWithAnInvalidAuthor(string userName, string email)
    {
        // Arrange
        var author = new Author()
        {
            UserName = userName,
            Email = email,
            Cheeps = new List<Cheep>()
        };
        var cheep = new CheepDTO()
        {
            Text = "test text",
            Author = author,
            TimeStamp = DateTime.Now
        };
        // Act
        
        // Assert
        Assert.Throws<ArgumentException>(()=>_cheepService.CreateCheep(cheep));
    }

    [Theory]
    [InlineData(
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.")]
    public void TestCheepCannotBeLongerThan160Characters(string text)
    {
        // Arrange
        var author = new Author()
        {
            UserName = "testname",
            Email = "userName@test.dk",
            Cheeps = new List<Cheep>()
        };
        var cheep = new CheepDTO()
        {
            Text = text,
            Author = author,
            TimeStamp = DateTime.Now
        };
        // Act

        // Assert
        Assert.Throws<ArgumentException>(()=>_cheepService.CreateCheep(cheep));
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
        _client.BaseAddress = new Uri("https://bdsagroup9chirprazor.azurewebsites.net/");
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