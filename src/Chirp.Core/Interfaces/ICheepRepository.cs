using Chirp.Core.Classes;

public interface ICheepRepository {
    public void CreateCheep(CheepDTO newCheep);
    public List<CheepDTO> ReadCheeps();
	public List<CheepDTO> ReadCheeps(int authorId);
    public List<CheepDTO> ReadCheeps(string authorName);
	public Author GetAuthor(int authorId);
    public Author GetAuthor(string authorName);
    public Author GetAuthorByEmail(string email);
    public void CreateAuthor(Author newAuthor);
    public void UpdateCheep(CheepDTO newCheep);
}