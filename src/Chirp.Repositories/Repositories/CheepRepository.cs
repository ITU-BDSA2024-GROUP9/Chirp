namespace Chirp.Repositories.Repositories;

using Chirp.Core.Classes;
using Chirp.Core.DTO;
using Chirp.Repositories.Helpers;
using Chirp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class CheepRepository : ICheepRepository
{
	private readonly ChirpDBContext _dbContext;
	public CheepRepository(ChirpDBContext dBContext)
	{
		_dbContext = dBContext;
	}
	
	/// <summary>
	/// Creates a new Cheep and associates it with an existing Author.
	/// </summary>
	/// <param name="newCheep">The CheepDTO object containing the details of the new Cheep to be created.</param>
	/// <returns>The ID of the newly created Cheep.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when the Cheep text is longer than 160 characters or when the Author is not found.
	/// </exception>
	public int CreateCheep(CheepDTO newCheep)
	{
		if (newCheep.Text.Length > 160)
		{
			throw new ArgumentException("Cheep text cannot be longer than 160 characters.");
		}

		var foundAuthor = GetAuthorByID(newCheep.Author.Id);
		if (foundAuthor == null)
		{
			throw new ArgumentException("Author not found.");
		}
		else foundAuthor.Cheeps ??= [];

		var cheep = new Cheep
		{
			Author = ToDomain(foundAuthor),
			AuthorId = foundAuthor.Id,
			Text = newCheep.Text,
			TimeStamp = DateTime.Now,
			Images = newCheep.Images
		};

		foundAuthor.Cheeps.Add(cheep);

		_dbContext.SaveChanges();
		return cheep.CheepId;
	}
	
	/// <summary>
	/// Used for getting the amount of cheeps in the database
	/// </summary>
	/// <returns>Amount of cheeps as an int</returns>
	public int GetCheepCount()
	{
		return _dbContext.Cheeps.Count();
	}

	public int GetCheepCountByID(string authorId)
	{
		return _dbContext.Cheeps
			.Include(c => c.Author)
			.Count(c => c.Author.Id == authorId);
	}

	public int GetCheepCountByName(string authorName)
	{
		return _dbContext.Cheeps
			.Include(c => c.Author)
			.Count(c => EF.Functions.Collate(c.Author.UserName, "NOCASE") == authorName);
	}

	public int GetCheepCountByAuthors(List<AuthorDTO> followedAuthors, string currentUserId)
	{
		followedAuthors ??= [];

		var authorIds = followedAuthors.Select(a => a.Id).ToList();
		if (!authorIds.Contains(currentUserId))
		{
			authorIds.Add(currentUserId);
		}

		// Return the count of cheeps where the author ID is in the list of author IDs
		return _dbContext.Cheeps
			.Include(c => c.Author)
			.Count(c => authorIds.Contains(c.Author.Id));
	}

	public List<CheepDTO> GetCheeps(int page)
	{
		var cheeps = _dbContext.Cheeps
			.Include(c => c.Author)
			.OrderByDescending(c => c.TimeStamp)
			.Skip((page - 1) * 32)
			.Take(32)
			.ToList();
		return cheeps.Select(c => CheepMapper.toDTO(c)).ToList();
	}

	public List<AuthorDTO> getFollowedInCheeps(AuthorDTO followerDTO)
	{
		var follower = ToDomain(followerDTO);
		var authors = _dbContext.Follows
			.Where(f => f.Follower == follower)
			.Select(f => f.Followed)
			.Where(a => a != null)
			.Distinct()
			.ToList()!;


		return authors.Select(a => AuthorMapper.toDTO(a!)).ToList();
	}
	public List<CheepDTO> GetCheepsFromAuthorByName(string authorName, int page)
	{
		var cheeps = _dbContext.Cheeps
			.Include(c => c.Author)
			.Where(c => EF.Functions.Collate(c.Author.UserName, "NOCASE") == authorName)
			.OrderByDescending(c => c.TimeStamp)
			.Skip((page - 1) * 32)
			.Take(32)
			.ToList();
		return cheeps.Select(c => CheepMapper.toDTO(c)).ToList();
	}

	public List<CheepDTO> GetCheepsFromAuthors(List<AuthorDTO> followedAuthors, string currentUserID, int pageNumber)
	{
		var authorIds = followedAuthors.Select(a => a.Id).ToList();
		authorIds.Add(currentUserID);

		var cheeps = _dbContext.Cheeps
			.Include(c => c.Author)
			.Where(c => authorIds.Contains(c.Author.Id))
			.OrderByDescending(c => c.TimeStamp)
			.Skip((pageNumber - 1) * 32)
			.Take(32)
			.ToList();
		return cheeps.Select(c => CheepMapper.toDTO(c)).ToList();
	}

	public bool IsFollowing(AuthorDTO followerAuthor, AuthorDTO followedAuthor)
	{
		return followerAuthor.Following.Any(f => f.FollowedId == followedAuthor.Id);
	}

	public void Follow(AuthorDTO followerAuthor, AuthorDTO followedAuthor)
	{
		var existingFollow = _dbContext.Follows
			.FirstOrDefault(f => f.FollowerId == followerAuthor.Id && f.FollowedId == followedAuthor.Id);

		if (existingFollow != null)
		{
			Console.WriteLine($"Follow relationship already exists: {followerAuthor.Id} -> {followedAuthor.Id}");
			return;
		}

		var followEntry = new Follow
		{
			Followed = ToDomain(followedAuthor),
			Follower = ToDomain(followerAuthor),
			FollowedId = followedAuthor.Id,
			FollowerId = followerAuthor.Id
		};

		_dbContext.Follows.Add(followEntry);
		_dbContext.SaveChanges();
	}
	public void Unfollow(AuthorDTO followerAuthor, AuthorDTO followedAuthor)
	{
		var followEntry = _dbContext.Follows
			.FirstOrDefault(f => f.FollowerId == followerAuthor.Id && f.FollowedId == followedAuthor.Id);

		if (followEntry == null)
		{
			throw new ArgumentException("Follow relationship does not exist.");
		}

		_dbContext.Follows.Remove(followEntry);
		_dbContext.SaveChanges();
	}



	public List<CheepDTO> GetCheepsFromAuthorByID(string authorID, int page)
	{
		var cheeps = _dbContext.Cheeps
			.Include(c => c.Author)
			.Where(c => c.Author.Id == authorID)
			.OrderByDescending(c => c.TimeStamp)
			.Skip((page - 1) * 32)
			.Take(32)
			.ToList();
		return cheeps.Select(c => CheepMapper.toDTO(c)).ToList();
	}

	public CheepDTO GetCheepByID(int cheepID)
	{
		var cheep = _dbContext.Cheeps
			.Include(c => c.Author)
			.FirstOrDefault(c => c.CheepId == cheepID);
		return cheep == null ? throw new ArgumentException("Cheep not found.") : CheepMapper.toDTO(cheep);
	}

	public void DeleteCheep(int cheepID)
	{
		var cheep = _dbContext.Cheeps.Find(cheepID);
		if (cheep != null)
		{
			_dbContext.Cheeps.Remove(cheep);
			_dbContext.SaveChanges();
		}
	}

	public AuthorDTO? GetAuthorByID(string authorId)
	{
		var author = _dbContext.Authors
			.Include(a => a.Cheeps) // Eager loading
			.FirstOrDefault(a => a.Id == authorId);
		return author == null ? null : AuthorMapper.toDTO(author);
	}

	public AuthorDTO? GetAuthorByName(string authorName)
	{
		var author = _dbContext.Authors
			.Include(a => a.Cheeps) // Eager loading
			.FirstOrDefault(a => EF.Functions.Collate(a.UserName, "NOCASE") == authorName);
		return author == null ? null : AuthorMapper.toDTO(author);
	}

	public AuthorDTO? GetAuthorByEmail(string email)
	{
		var author = _dbContext.Authors
			.Include(a => a.Cheeps) // Eager loading
			.FirstOrDefault(a => a.Email == email);
		return author == null ? throw new ArgumentException("Author not found!") : AuthorMapper.toDTO(author);
	}

	public Author ToDomain(AuthorDTO author)
	{
		return AuthorMapper.toDomain(author, _dbContext);
	}

	public void CreateAuthor(AuthorDTO newAuthor)
	{
		_dbContext.Authors.Add(ToDomain(newAuthor));
		_dbContext.SaveChanges();
	}

	public void UpdateCheep(CheepDTO newCheep, int cheepID)
	{
		var cheep = _dbContext.Cheeps.Find(cheepID);
		if (cheep != null)
		{
			cheep.Text = newCheep.Text;
			_dbContext.SaveChanges();
		}
	}

	public List<CommentDTO> GetCommentsForCheep(int cheepId)
	{
		var comments = _dbContext.Comments
			.Include(c => c.Author)
			.Where(c => c.CheepId == cheepId)
			.OrderBy(c => c.TimeStamp)
			.ToList();
		return comments.Select(c => CommentMapper.toDTO(c)).ToList();
	}

	public int GetCommentCountForCheep(int cheepId)
	{
		return _dbContext.Comments.Count(c => c.CheepId == cheepId);
	}

	public void AddComment(CommentDTO comment)
	{
		var cheep = _dbContext.Cheeps.Find(comment.CheepId);
		if (cheep == null)
		{
			throw new ArgumentException("Cheep not found.");
		}

		// Retrieve the tracked Author instance from the DbContext
		var trackedAuthor = _dbContext.Authors.Find(comment.Author.Id);
		if (trackedAuthor == null)
		{
			throw new ArgumentException("Author not found.");
		}

		var newComment = new Comment
		{
			Text = comment.Text,
			TimeStamp = DateTime.Now,
			Author = trackedAuthor, // Use the tracked Author instance
			AuthorId = trackedAuthor.Id,
			Cheep = cheep,
			CheepId = cheep.CheepId
		};

		_dbContext.Comments.Add(newComment);
		_dbContext.SaveChanges();
	}

	/// <summary>
	/// Deletes a specific comment using its ID
	/// </summary>
	/// <param name="commentId"></param>
	public void DeleteComment(int commentId)
	{
		var comment = _dbContext.Comments.Find(commentId);
		if (comment != null)
		{
			_dbContext.Comments.Remove(comment);
			_dbContext.SaveChanges();
		}
	}
}