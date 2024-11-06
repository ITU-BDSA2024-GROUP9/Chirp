using Chirp.Core.Classes;

public interface ICheepRepository {
    public int CreateCheep(CheepDTO newCheep);
    public int GetCheepCount();
    public int GetCheepCountByID(string authorId);
    public int GetCheepCountByName(string authorName); 
    public List<CheepDTO> ReadCheeps();
	public List<CheepDTO> ReadCheepsByID(string authorId);
    public List<CheepDTO> ReadCheepsByName(string authorName);
    public List<CheepDTO> ReadCheeps(int page);
	public List<CheepDTO> ReadCheepsByID(string authorId, int page);
    public List<CheepDTO> ReadCheepsByName(string authorName, int page);
	public Author? GetAuthorByID(string authorId);
    public Author? GetAuthorByName(string authorName);
    public Author? GetAuthorByEmail(string email);
    public void CreateAuthor(Author newAuthor);
    public void UpdateCheep(CheepDTO newCheep, int cheepID);
}