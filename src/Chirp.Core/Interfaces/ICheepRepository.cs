using Chirp.Core.Classes;

public interface ICheepRepository {
    public int CreateCheep(CheepDTO newCheep);
    public int GetCheepCount();
    public int GetCheepCountByID(string authorId);
    public int GetCheepCountByName(string authorName); 
    public List<CheepDTO> GetCheepsFromAuthorByID(string authorId, int page);
    public bool IsFollowing(Author followerAuthor, Author followedAuthor);
    public void Follow(Author followerAuthor, Author followedAuthor);
    public void Unfollow(Author followerAuthor, Author followedAuthor);

    public List<Author> getFollowedInCheeps(Author follower);
    public List<CheepDTO> GetCheepsFromAuthors(List<Author> followedAuthors, int pageNumber);
    public List<CheepDTO> GetCheepsFromAuthorByName(string authorName, int page);        
    public List<CheepDTO> GetCheeps(int page);
    public CheepDTO GetCheepByID(int cheepID);
	public Author? GetAuthorByID(string authorId);
    public Author? GetAuthorByName(string authorName);
    public Author? GetAuthorByEmail(string email);
    public void CreateAuthor(Author newAuthor);
    public void UpdateCheep(CheepDTO newCheep, int cheepID);
    public void DeleteCheep(int cheepID);
}