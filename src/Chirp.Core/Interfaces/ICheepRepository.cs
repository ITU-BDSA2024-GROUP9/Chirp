using Chirp.Core.Classes;

public interface ICheepRepository {
    public void CreateCheep(CheepDTO newCheep);
    public List<CheepDTO> ReadCheeps();
	public List<CheepDTO> ReadCheeps(int authorId);
	public Author GetAuthor(int authorId);
    public void UpdateCheep(CheepDTO newCheep);
}