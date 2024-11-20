using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Chirp.Core.Classes;
using Chirp.Core.Helpers;
using Chirp.Core.Interfaces;
using Chirp.Razor.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using Assert = Xunit.Assert;

namespace Chirp.Razor.Tests;

// ref: https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database 
public class TestDatabaseFixture : IDisposable
{
    public readonly SqliteConnection ConnectionString;
    public readonly List<Author> Authors;

    public TestDatabaseFixture()
    {
        // Create an in-memory SQLite connection
        ConnectionString = new SqliteConnection("Filename=:memory:");
        ConnectionString.Open();

        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(ConnectionString)
            .Options;

        var context = new ChirpDBContext(options);

        // Ensure the schema is created, including Identity tables (via migrations)
        context.Database.EnsureCreated();  // This will create tables from any migrations applied

        // Optionally, apply pending migrations if necessary
        context.Database.Migrate();  // Apply any pending migrations to ensure all tables (including Identity) are created

        // Seed the database with initial data, if necessary
        Authors = DbInitializer.SeedDatabase(context);
        // TODO - run dbinitializer.setpasswords() somehow
    }

    public ChirpDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(ConnectionString)
            .Options;

        return new ChirpDBContext(options);
    }

    public void Dispose()
    {
        ConnectionString.Dispose();  // Cleanup
    }
}


[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class UITests : PageTest
{
    [SetUp]
    public async Task Init()
    {
        await MyEndToEndUtil.StartServer(); // Starts the server before each test
    }

    [Test]
    public async Task TestRegisterAndLogin()
    {
        await Page.GotoAsync("http://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test1!");
        await Page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
        await Page.GetByLabel("Confirm Password").FillAsync("Test1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByPlaceholder("password").FillAsync("Test1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }
    
    [Test]
    public async Task TestAccessUserTimelineAsAnonymousUser()
    {
        await Page.GotoAsync("http://localhost:5273/");
        await Page.Locator("body").ClickAsync();
        await Page.Locator("p").Filter(new() { HasText = "Wendell Ballan At first he" }).GetByRole(AriaRole.Link).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "→" }).ClickAsync();
        await Page.GetByText("Wendell Ballan No great and").ClickAsync();
    }

    [Test]
    public async Task UserRegistersAndAccessesUserTimeline()
    {
        await Page.GotoAsync("http://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test1!");
        await Page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
        await Page.GetByLabel("Confirm Password").FillAsync("Test1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByPlaceholder("password").FillAsync("Test1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.Locator("p").Filter(new() { HasText = "Jacqualine Gilcoine And then" }).GetByRole(AriaRole.Link).ClickAsync();
        await Page.GetByText("Jacqualine Gilcoine In various enchanted attitudes, like the Sperm Whale. — 01/").ClickAsync();
    }

    [Test]
    public async Task UserRegistersAndPostsCheepAndAccessesPrivateTimeline()
    {
        await Page.GotoAsync("http://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test1!");
        await Page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
        await Page.GetByLabel("Confirm Password").FillAsync("Test1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test");
        await Page.GetByPlaceholder("name@example.com").PressAsync("ArrowLeft");
        await Page.GetByPlaceholder("name@example.com").PressAsync("ArrowRight");
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByPlaceholder("password").FillAsync("Test1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByPlaceholder("Type here!").ClickAsync();
        await Page.GetByPlaceholder("Type here!").FillAsync("Hi everyone!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
        await Page.GetByText("test@mail.com Hi everyone! —").ClickAsync();
    }



    [TearDown]
    public void Cleanup()
    {
        MyEndToEndUtil.StopServer(); // Stops the server after each test
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

    [Xunit.Theory]
    [InlineData("Hej med dig smukke", "11")]
    public void TestCreateCheeps(string text, string authorID)
    {
        // Arrange
        var author = _cheepService.GetAuthorByID(authorID);
        if (author == null)
        {
            throw new ArgumentException("Author does not exist");
        }
        
        var cheep = new CheepDTO()
        {
            Text = text,
            TimeStamp = DateTime.Now,
            Author = author
        };
        
        // Act
        _cheepService.CreateCheep(cheep);
        var result = _cheepService.GetCheepsFromAuthorByID(authorID, 1);
        
        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(result[0].Text, text);
    }

    [Xunit.Theory]
    [InlineData("11")] // Helge's ID
    public void RetrieveAllDataRelatedToAuthor(string ID)
    {
        // Arrange
        
        // Act
        var result = _cheepService.GetAuthorByID(ID);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("11", result.Id);
        Assert.Equal("ropf@itu.dk", result.Email);
        Assert.Equal("Helge", result.UserName);
        Assert.NotEmpty(result.Cheeps);
    }
    
    [Xunit.Theory]
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
        var result = _cheepService.GetCheepsFromAuthorByName(author.UserName, 1);
       
        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(result[0].Text, text);
    }
    
    [Xunit.Theory]
    [InlineData("11")]
    [InlineData("12")]
    public void TestGetCheeps(string id)
    {
        // Arrange
        var author = _cheepRepo.GetAuthorByID(id);
        if (author == null)
        {
            throw new ArgumentException("Author does not exist");
        }
        
        // Act
        List<CheepDTO> authorCheeps = _cheepRepo.GetCheepsFromAuthorByID(author.Id, 1);

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
        var originalResult = _cheepService.GetCheepsFromAuthorByID(author.Id, 1)[0].Text;
        _cheepService.UpdateCheep(updatedCheep, cheepID);
        var updatedResult = _cheepService.GetCheepsFromAuthorByID(author.Id, 1)[0].Text;
        // Assert
        Assert.Equal(originalText, originalResult);
        Assert.Equal(updatedText, updatedResult);
    }

    [Xunit.Theory]
    [InlineData("1")]
    [InlineData("2")]
    public void TestGetCheepsFromAuthorWithID(string authorId)
    {
        // Arrange
        
        // Act
        var result = _cheepService.GetCheepsFromAuthorByID(authorId, 1);
        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(result[0].Author.Id, authorId);
    }

    [Xunit.Theory]
    [InlineData("Helge")]
    public void TestGetCheepsFromAuthorWithName(string name)
    {
        // Arrange
        
        // Act
        var result = _cheepService.GetCheepsFromAuthorByName(name, 1);
        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(result[0].Author.UserName, name);
    }

    [Xunit.Theory]
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
    
    [Xunit.Theory]
    [InlineData("11", "Helge")]
    public void TestGetAuthorWithName(string id, string userName)
    {
        //Act
        var result = _cheepService.GetAuthorByName(userName);
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }
    
    [Xunit.Theory]
    [InlineData("11", "ropf@itu.dk")]
    public void TestGetAuthorWithEmail(string id, string email)
    {
        //Act
        var result = _cheepService.GetAuthorByEmail(email);
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Xunit.Theory]
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
    [Xunit.Theory]
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

    [Xunit.Theory]
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
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<TestDatabaseFixture>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;
    private readonly TestDatabaseFixture _testDatabaseFixture;

    // The constructor should accept both WebApplicationFactory<Program> and TestDatabaseFixture
    public IntegrationTests(WebApplicationFactory<Program> fixture, TestDatabaseFixture testDatabaseFixture)
    {
        _testDatabaseFixture = testDatabaseFixture;

        // Customize the web host to use the test-specific configuration
        _fixture = fixture.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration (if it exists)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ChirpDBContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Register the test-specific DbContext using the fixture's connection string
                services.AddDbContext<ChirpDBContext>(options =>
                {
                    options.UseSqlite(_testDatabaseFixture.ConnectionString);
                });

                // Ensure migrations are applied before tests run
                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
                    dbContext.Database.Migrate();  // Apply migrations if necessary
                }
            });
        });

        // Initialize HttpClient
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true
        });
    }


    // This test is from https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
    [Xunit.Theory]
    [InlineData("/")]
    public async Task Get_EndpointsReturnSuccess(string url)
    {
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
    }
    
    // This was based on https://github.com/itu-bdsa/lecture_notes/blob/main/sessions/session_05/Slides.md#testing-of-web-applications--integration-testing-1
    [Xunit.Theory]
    [InlineData("Jacqualine Gilcoine")]
    [InlineData("Quintin Sitts")]
    public async void CanSeePrivateTimelineAzure(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        //Assert.Fail(content);
        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);

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

    [Xunit.Theory]
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

    [Xunit.Theory]
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

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class EndToEndTests : PageTest
{

    // dissable nullable warnings since the fields are initialized in the setup method
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private TestDatabaseFixture _fixture;
    private CheepRepository _cheepRepo;
    private ChirpDBContext _context;
    private CheepService _cheepService;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    [SetUp]
    public async Task Init()
    {
        _fixture = new TestDatabaseFixture();
        _context = _fixture.CreateContext();
        _cheepRepo = new CheepRepository(_context);
        _cheepService = new CheepService(_cheepRepo);
        await MyEndToEndUtil.StartServer(); // Starts the server before each test
    }

    [Test]
    public async Task E2ETest()
    {
        //User registers for the first time and accesses the homepage.
        await Page.GotoAsync("http://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Test1!");
        await Page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
        await Page.GetByLabel("Confirm Password").FillAsync("Test1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        //User logs in.
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
        await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await Page.GetByPlaceholder("password").FillAsync("Test1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        //User posts a cheep.
        await Page.GetByPlaceholder("Type here!").ClickAsync();
        await Page.GetByPlaceholder("Type here!").FillAsync("Hello, everyone!");
        await Page.GetByPlaceholder("Type here!").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        //User goes to their timeline and confirms their cheep is there.
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
        await Page.GetByText("test@mail.com Hello, everyone").ClickAsync();

        //The database has registered the user as well as the cheep.
        var result = _cheepService.GetAuthorByEmail("test@mail.com");
        Assert.NotNull(result);

        //Get cheeps from the author.
        var cheeps = _cheepService.GetCheepsFromAuthorByID(result.Id, 1);

        //Assert that the cheep exists in the database
        bool cheepExists = false;
        foreach (CheepDTO cheep in cheeps)
        {
            if (cheep.Text.Equals("Hello, everyone!"))
            {
                cheepExists = true;
            }
        }

        Assert.True(cheepExists);

        //User goes to the public timeline and clicks on a user which isn't them.
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Jacqualine Gilcoine" }).ClickAsync();

        //User follows the user.
        await Page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();

        //User goes to their timeline and confirms the user they followed is there.
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Jacqualine Gilcoine" })).ToBeVisibleAsync();

        //User unfollows the user.
        await Page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" }).ClickAsync();

        //User goes to their timeline and confirms the user they unfollowed is not there.
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Jacqualine Gilcoine" })).Not.ToBeVisibleAsync();
    }


    [TearDown]
    public void Cleanup()
    {
        MyEndToEndUtil.StopServer(); // Stops the server after each test
        _context.Dispose();
        _fixture.Dispose();
    }

    class TestUtils
    {
        public static string
            UnixTimeStampToDateTimeString(
                double unixTimeStamp) // TODO - Make a util class for this method and use it here and in DBFacade
        {
            // Unix timestamp is seconds past epoch
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
    }
}