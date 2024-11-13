using Chirp.Core.Classes;

public interface ICheepRepository {
    public int CreateCheep(CheepDTO newCheep);
    public int GetCheepCount();
    public int GetCheepCountByID(string authorId);
    public int GetCheepCountByName(string authorName); 
    public List<CheepDTO> GetCheepsFromAuthorByID(string authorId, int page);
    public List<CheepDTO> GetCheepsFromFollowed(string authorId);

    public bool IsFollowing(Author followerAuthor, Author followedAuthor);


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