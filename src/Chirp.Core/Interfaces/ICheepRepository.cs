using Chirp.Core.Classes;

public interface ICheepRepository {
    public int CreateCheep(CheepDTO newCheep);
    public List<CheepDTO> ReadCheeps();
	public List<CheepDTO> ReadCheepsByID(string authorId);
    public List<CheepDTO> ReadCheepsByName(string authorName);
	public Author? GetAuthorByID(string authorId);
    public Author? GetAuthorByName(string authorName);
    public Author? GetAuthorByEmail(string email);
    public void CreateAuthor(Author newAuthor);
    public void UpdateCheep(CheepDTO newCheep, int cheepID);
}