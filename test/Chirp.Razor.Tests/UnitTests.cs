using Chirp.Core.Classes;
using Chirp.Core.Helpers;
using Chirp.Razor.Services;

namespace Chirp.Razor.Tests;


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
            Cheeps = []
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
            Cheeps = []
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
    [InlineData("69420", "John Doe", "johndoe@yahoo.com")]
    public void TestCreateAuthor(string id, string newAuthor, string email){
        // arrange
        var author = new Author
        {
            Cheeps = [],
            Id = id,
            UserName = newAuthor,
            Email = email
        };

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
            Cheeps = []
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
            Cheeps = []
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