namespace Chirp.Core.Classes;

using System.Threading.Tasks;
using Chirp.Core.Helpers;
using Microsoft.EntityFrameworkCore;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    public CheepRepository(ChirpDBContext dBContext)
    {
        _dbContext = dBContext;
    }

    public void CreateCheep(CheepDTO newCheep)
    {
        var foundAuthor = _dbContext.Authors.FirstOrDefault(a => a.Name == newCheep.Author.Name);
        if (foundAuthor == null)
        {
            _dbContext.Authors.Add(new Author
            {
                Name = newCheep.Author.Name,
                Email = newCheep.Author.Email,
                Cheeps = []
            });
        }
        else
        {
            foundAuthor.Cheeps.Add(new Cheep
            {
                Author = foundAuthor,
                AuthorId = foundAuthor.AuthorId,
                Text = newCheep.Text,
                TimeStamp = DateTime.Now
            });
        }
        _dbContext.SaveChanges();
    }

    public List<CheepDTO> ReadCheeps(int authorId)
    {
        var cheeps = _dbContext.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.AuthorId == authorId)
            .Select(c => new CheepDTO
            {
                Text = c.Text,
                TimeStamp = c.TimeStamp,
                Author = c.Author
            })
            .ToList();
        return cheeps;
    }

    public List<CheepDTO> ReadCheeps(string authorName)
    {
        var cheeps = _dbContext.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == authorName)
            .Select(c => new CheepDTO
            {
                Text = c.Text,
                TimeStamp = c.TimeStamp,
                Author = c.Author
            })
            .ToList();
        return cheeps;
    }

    public List<CheepDTO> ReadCheeps()
    {
        var cheeps = _dbContext.Cheeps
            .Include(c => c.Author)
            .Select(c => new CheepDTO
            {
                Text = c.Text,
                TimeStamp = c.TimeStamp,
                Author = c.Author
            })
            .ToList();
        return cheeps;
    }

    public Author GetAuthor(int authorId)
    {
        return _dbContext.Authors.FirstOrDefault(a => a.AuthorId == authorId);
    }

    public Author GetAuthor(string authorName)
    {
        return _dbContext.Authors.FirstOrDefault(a => a.Name == authorName);
    }

    public Author GetAuthorByEmail(string email)
    {
        return _dbContext.Authors.FirstOrDefault(a => a.Email == email);
    }

    public void CreateAuthor(Author newAuthor)
    {
        _dbContext.Authors.Add(newAuthor);
        _dbContext.SaveChanges();
    }

    public void UpdateCheep(CheepDTO newCheep)
    {
        var cheep = _dbContext.Cheeps
            .Include(c => c.Author)
            .FirstOrDefault(c => c.Author.Name == newCheep.Author.Name && c.TimeStamp == newCheep.TimeStamp);
        if (cheep != null)
        {
            cheep.Text = newCheep.Text;
            _dbContext.SaveChanges();
        }
    }
}