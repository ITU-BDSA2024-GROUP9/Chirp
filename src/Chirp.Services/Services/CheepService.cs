using Chirp.Core.Classes;
using Chirp.Core.DTO;
using Chirp.Repositories.Interfaces;
using Chirp.Services.Interfaces;

namespace Chirp.Services
{
	public class CheepService : ICheepService
	{
		private readonly ICheepRepository _repository;


		public CheepService(ICheepRepository repository)
		{
			_repository = repository;
		}
		
		/// <summary>
		/// Used for getting all the cheeps from a specific page
		/// </summary>
		/// <param name="page"></param>
		/// <returns>A list of cheep DTO from the specific page</returns>
		public List<CheepDTO> GetCheeps(int page)
		{
			return _repository.GetCheeps(page);
		}
		
		/// <summary>
		/// Used for getting the all the cheeps from an author, by looking up their ID
		/// </summary>
		/// <param name="authorID"></param>
		/// <param name="page"></param>
		/// <returns>A list of cheeps from the specified user</returns>
		public List<CheepDTO> GetCheepsFromAuthorByID(string authorId, int page)
		{
			return _repository.GetCheepsFromAuthorByID(authorId, page);
		}
		
		/// <summary>
		/// Used for getting the cheeps by a specific author using the author's name for lookup
		/// </summary>
		/// <param name="authorName"></param>
		/// <param name="page"></param>
		/// <returns>A list of cheeps made by the specified author</returns>
		public List<CheepDTO> GetCheepsFromAuthorByName(string authorName, int page)
		{
			return _repository.GetCheepsFromAuthorByName(authorName, page);
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
			return _repository.CreateCheep(newCheep);
		}
		
		/// <summary>
		/// Used for checking if an author is following another author
		/// </summary>
		/// <param name="followerAuthor"></param>
		/// <param name="followedAuthor"></param>
		/// <returns>True or false depending on if the author is following the other author</returns>
		public bool IsFollowing(AuthorDTO followerAuthor, AuthorDTO followedAuthor)
		{
			return _repository.IsFollowing(followerAuthor, followedAuthor);
		}
		
		/// <summary>
		/// Used for getting a list of the authors an author is following
		/// </summary>
		/// <param name="followerDTO"></param>
		/// <returns>a list of the authors an author is following</returns>
		public List<AuthorDTO> getFollowedInCheeps(AuthorDTO follower)
		{
			return _repository.getFollowedInCheeps(follower);
		}

		/// <summary>
		/// Used for getting the cheeps by a specific author using the author's ID for lookup
		/// </summary>
		/// <param name="followedAuthors"></param>
		/// <param name="currentUserID"></param>
		/// <param name="pageNumber"></param>
		/// <returns>A list of cheeps made by the specified author</returns>
		public List<CheepDTO> GetCheepsFromAuthors(List<AuthorDTO> followedAuthors, string currentUserID, int pageNumber)
		{
			return _repository.GetCheepsFromAuthors(followedAuthors, currentUserID, pageNumber);
		}
		
		/// <summary>
		/// Used for following another user
		/// </summary>
		/// <param name="followerAuthor"></param>
		/// <param name="followedAuthor"></param>
		public void Follow(AuthorDTO followerAuthor, AuthorDTO followedAuthor)
		{
			_repository.Follow(followerAuthor, followedAuthor);
		}

		/// <summary>
		/// Used for unfollowing another user
		/// </summary>
		/// <param name="followerAuthor"></param>
		/// <param name="followedAuthor"></param>
		/// <exception cref="ArgumentException"></exception>
		public void Unfollow(AuthorDTO followerAuthor, AuthorDTO followedAuthor)
		{
			_repository.Unfollow(followerAuthor, followedAuthor);
		}

		/// <summary>
		/// Used for getting an author object by its ID
		/// </summary>
		/// <param name="authorId"></param>
		/// <returns>The author with the specified ID</returns>
		public AuthorDTO? GetAuthorByID(string authorId)
		{
			return _repository.GetAuthorByID(authorId);
		}

		/// <summary>
		/// Used for getting an author object by its name
		/// </summary>
		/// <param name="authorName"></param>
		/// <returns>The author with the specified name</returns>
		public AuthorDTO? GetAuthorByName(string authorName)
		{
			return _repository.GetAuthorByName(authorName);
		}

		/// <summary>
		/// Used for getting an author object by its email
		/// </summary>
		/// <param name="email"></param>
		/// <returns>The author with the specified name</returns>
		/// <exception cref="ArgumentException"></exception>
		public AuthorDTO? GetAuthorByEmail(string email)
		{
			return _repository.GetAuthorByEmail(email);
		}

		public int GetCheepCount()
		{
			return _repository.GetCheepCount();
		}

		public int GetCheepCountByID(string authorId)
		{
			return _repository.GetCheepCountByID(authorId);
		}

		public int GetCheepByName(string authorName)
		{
			return _repository.GetCheepCountByName(authorName);
		}

		public Author ToDomain(AuthorDTO author)
		{
			return _repository.ToDomain(author);
		}

		public int GetCheepCountByAuthors(List<AuthorDTO> followedAuthors, string currentUserId)
		{
			return _repository.GetCheepCountByAuthors(followedAuthors, currentUserId);
		}

		public void UpdateCheep(CheepDTO newCheep, int cheepID)
		{
			_repository.UpdateCheep(newCheep, cheepID);
		}

		public void DeleteCheep(int cheepId)
		{
			_repository.DeleteCheep(cheepId);
		}

		public CheepDTO GetCheepByID(int cheepId)
		{
			return _repository.GetCheepByID(cheepId);
		}

		public List<CommentDTO> GetCommentsForCheep(int cheepId)
		{
			return _repository.GetCommentsForCheep(cheepId);
		}

		public int GetCommentCountForCheep(int cheepId)
		{
			return _repository.GetCommentsForCheep(cheepId).Count;
		}

		public void AddComment(CommentDTO comment)
		{
			_repository.AddComment(comment);
		}

		public void DeleteComment(int commentId)
		{
			_repository.DeleteComment(commentId);
		}
	}
}