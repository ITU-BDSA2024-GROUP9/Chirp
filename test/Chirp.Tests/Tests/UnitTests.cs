using Chirp.Core.Classes;
using Chirp.Core.DTO;
using Chirp.Repositories.Helpers;
using Chirp.Repositories.Repositories;
using Chirp.Services;
using Chirp.Tests.Helpers;

namespace Chirp.Tests.Tests
{
	public class UnitTests : IDisposable
	{
		private readonly InMemoryDatabaseHelper _fixture;
		private readonly CheepRepository _cheepRepo;
		private readonly ChirpDBContext _context;
		private readonly CheepService _cheepService;

		public UnitTests()
		{
			_fixture = new InMemoryDatabaseHelper();
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
				Author = _cheepService.ToDomain(author)
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
			_cheepRepo.CreateAuthor(AuthorMapper.toDTO(author));
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
			_cheepRepo.CreateAuthor(AuthorMapper.toDTO(author));
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
		public void TestCreateAuthor(string id, string newAuthor, string email)
		{
			// arrange
			var author = new Author
			{
				Cheeps = [],
				Id = id,
				UserName = newAuthor,
				Email = email
			};

			// act
			_cheepRepo.CreateAuthor(AuthorMapper.toDTO(author));

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
			Assert.Throws<ArgumentException>(() => _cheepService.CreateCheep(cheep));
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
			Assert.Throws<ArgumentException>(() => _cheepService.CreateCheep(cheep));
		}

		[Xunit.Theory]
		[InlineData("11")]
		public void TestGettingCheepCountByTheID(string id)
		{
			// Arrange
			// Act
			var count = _cheepRepo.GetCheepCountByID(id);
			// Assert
			Assert.Equal(1, count);
		}

		[Fact]
		public void TestGetCheepCountByAuthors()
		{
			// Arrange
			var author1 = new Author()
			{
				Id = "author1",
				UserName = "Author One",
				Email = "author1@test.com",
				Cheeps = new List<Cheep>()
			};

			var author2 = new Author()
			{
				Id = "author2",
				UserName = "Author Two",
				Email = "author2@test.com",
				Cheeps = new List<Cheep>()
			};

			var currentUser = new Author()
			{
				Id = "currentUserId",
				UserName = "Current User",
				Email = "currentuser@test.com",
				Cheeps = new List<Cheep>()
			};

			_cheepRepo.CreateAuthor(AuthorMapper.toDTO(author1));
			_cheepRepo.CreateAuthor(AuthorMapper.toDTO(author2));
			_cheepRepo.CreateAuthor(AuthorMapper.toDTO(currentUser));

			var cheep1 = new CheepDTO()
			{
				Text = "First cheep",
				TimeStamp = DateTime.Now,
				Author = author1
			};

			var cheep2 = new CheepDTO()
			{
				Text = "Second cheep",
				TimeStamp = DateTime.Now,
				Author = author2
			};

			var cheep3 = new CheepDTO()
			{
				Text = "Third cheep",
				TimeStamp = DateTime.Now,
				Author = currentUser
			};

			_cheepService.CreateCheep(cheep1);
			_cheepService.CreateCheep(cheep2);
			_cheepService.CreateCheep(cheep3);

			var followedAuthors = new List<AuthorDTO>
			{
				new AuthorDTO { Id = author1.Id, Cheeps = new List<Cheep>() },
				new AuthorDTO { Id = author2.Id, Cheeps = new List<Cheep>() }
			};

			// Act
			var count = _cheepRepo.GetCheepCountByAuthors(followedAuthors, currentUser.Id);

			// Assert
			Assert.Equal(3, count);
		}
		[Fact]
		public void TestGetFollowedInCheeps()
		{
			// Arrange
			var follower = new AuthorDTO()
			{
				Id = "followerId",
				UserName = "Follower",
				Email = "follower@test.com",
				Cheeps = new List<Cheep>()
			};

			var followed1 = new AuthorDTO()
			{
				Id = "followed1Id",
				UserName = "Followed One",
				Email = "followed1@test.com",
				Cheeps = new List<Cheep>()
			};

			var followed2 = new AuthorDTO()
			{
				Id = "followed2Id",
				UserName = "Followed Two",
				Email = "followed2@test.com",
				Cheeps = new List<Cheep>()
			};

			_cheepRepo.CreateAuthor(follower);
			_cheepRepo.CreateAuthor(followed1);
			_cheepRepo.CreateAuthor(followed2);

			_cheepRepo.Follow(follower, followed1);
			_cheepRepo.Follow(follower, followed2);

			// Act
			var followedAuthors = _cheepRepo.getFollowedInCheeps(follower);

			// Assert
			Assert.Contains(followedAuthors, a => a.Id == followed1.Id);
			Assert.Contains(followedAuthors, a => a.Id == followed2.Id);
			Assert.Equal(2, followedAuthors.Count);
		}
		[Fact]
		public void TestGetCheepsFromAuthors()
		{
			// Arrange
			var author1 = new Author()
			{
				Id = "author1",
				UserName = "Author One",
				Email = "author1@test.com",
				Cheeps = new List<Cheep>()
			};

			var author2 = new Author()
			{
				Id = "author2",
				UserName = "Author Two",
				Email = "author2@test.com",
				Cheeps = new List<Cheep>()
			};

			var currentUser = new Author()
			{
				Id = "currentUserId",
				UserName = "Current User",
				Email = "currentuser@test.com",
				Cheeps = new List<Cheep>()
			};

			_cheepRepo.CreateAuthor(AuthorMapper.toDTO(author1));
			_cheepRepo.CreateAuthor(AuthorMapper.toDTO(author2));
			_cheepRepo.CreateAuthor(AuthorMapper.toDTO(currentUser));

			var cheep1 = new CheepDTO()
			{
				Text = "First cheep",
				TimeStamp = DateTime.Now,
				Author = author1
			};

			var cheep2 = new CheepDTO()
			{
				Text = "Second cheep",
				TimeStamp = DateTime.Now,
				Author = author2
			};

			var cheep3 = new CheepDTO()
			{
				Text = "Third cheep",
				TimeStamp = DateTime.Now,
				Author = currentUser
			};

			_cheepService.CreateCheep(cheep1);
			_cheepService.CreateCheep(cheep2);
			_cheepService.CreateCheep(cheep3);

			var followedAuthors = new List<AuthorDTO>
			{
				new AuthorDTO { Id = author1.Id, Cheeps = new List<Cheep>() },
				new AuthorDTO { Id = author2.Id, Cheeps = new List<Cheep>() }
			};

			// Act
			var cheeps = _cheepRepo.GetCheepsFromAuthors(followedAuthors, currentUser.Id, 1);

			// Assert
			Assert.Equal(3, cheeps.Count);
			Assert.Contains(cheeps, c => c.Text == "First cheep");
			Assert.Contains(cheeps, c => c.Text == "Second cheep");
			Assert.Contains(cheeps, c => c.Text == "Third cheep");
		}
		
		[Fact]
		public void TestIsFollowing()
		{
			// Arrange
			var follower = new AuthorDTO
			{
				Id = "followerId",
				UserName = "Follower",
				Email = "follower@test.com",
				Following = new List<Follow>(),
				Cheeps = new List<Cheep>()
			};

			var followed = new AuthorDTO
			{
				Id = "followedId",
				UserName = "Followed",
				Email = "followed@test.com",
				Cheeps = new List<Cheep>()
				
			};

			_cheepRepo.Follow(follower, followed);

			// Act
			var isFollowing = _cheepRepo.IsFollowing(follower, followed);

			// Assert
			Assert.True(isFollowing);
		}
	}
}
