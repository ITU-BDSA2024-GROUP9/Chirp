using Chirp.Core.Classes;
using Chirp.Core.DTO;

namespace Chirp.Repositories.Interfaces;

public interface ICheepRepository
{
	public int CreateCheep(CheepDTO newCheep);
	public int GetCheepCount();
	public int GetCheepCountByID(string authorId);
	public int GetCheepCountByName(string authorName);
	public List<CheepDTO> GetCheepsFromAuthorByID(string authorId, int page);
	public int GetCheepCountByAuthors(List<AuthorDTO> followedAuthors, string currentUserId);
	public bool IsFollowing(AuthorDTO followerAuthor, AuthorDTO followedAuthor);
	public void Follow(AuthorDTO followerAuthor, AuthorDTO followedAuthor);
	public void Unfollow(AuthorDTO followerAuthor, AuthorDTO followedAuthor);

	public List<AuthorDTO> getFollowedInCheeps(AuthorDTO follower);
	public List<CheepDTO> GetCheepsFromAuthors(List<AuthorDTO> followedAuthors, string currentUserID, int pageNumber);
	public List<CheepDTO> GetCheepsFromAuthorByName(string authorName, int page);
	public List<CheepDTO> GetCheeps(int page);
	public CheepDTO GetCheepByID(int cheepID);
	public AuthorDTO? GetAuthorByID(string authorId);
	public AuthorDTO? GetAuthorByName(string authorName);
	public AuthorDTO? GetAuthorByEmail(string email);
	public void CreateAuthor(AuthorDTO newAuthor);
	public void UpdateCheep(CheepDTO newCheep, int cheepID);
	public void DeleteCheep(int cheepID);
	public Author ToDomain(AuthorDTO authorDTO);

	// Commenting-related methods
	List<CommentDTO> GetCommentsForCheep(int cheepId);
	void AddComment(CommentDTO comment);
	void DeleteComment(int commentId);
}