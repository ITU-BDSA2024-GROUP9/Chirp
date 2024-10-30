using Microsoft.AspNetCore.Authorization.Infrastructure;

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
        if (newCheep.Text.Length > 160)
        {
            throw new ArgumentException("Cheep text cannot be longer than 160 characters.");
        }
        var foundAuthor = _dbContext.Authors
            .Include(a => a.Cheeps) // Eager loading
            .FirstOrDefault(a => a.UserName == newCheep.Author.UserName);
        if (foundAuthor == null)
        {
            foundAuthor = new Author
            {
                UserName = newCheep.Author.UserName,
                Email = newCheep.Author.Email,
                Cheeps = new List<Cheep>()
            };
            _dbContext.Authors.Add(foundAuthor);
        }
        else if (foundAuthor.Cheeps == null)
        {
            foundAuthor.Cheeps = new List<Cheep>();
        }
        
        foundAuthor.Cheeps.Add(new Cheep
        {
            Author = foundAuthor,
            AuthorId = foundAuthor.Id,
            Text = newCheep.Text,
            TimeStamp = DateTime.Now
        });
    
        _dbContext.SaveChanges();
    }

    public List<CheepDTO> ReadCheeps(string authorId)
    {
        var cheeps = _dbContext.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Id == authorId)
            .Select(c => new CheepDTO
            {
                Text = c.Text,
                TimeStamp = c.TimeStamp,
                Author = c.Author
            })
            .ToList();
        
        return cheeps;
    }

    public List<CheepDTO> ReadCheepsByName(string authorName)
    {
        var cheeps = _dbContext.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.UserName == authorName)
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

    public Author GetAuthor(string authorId)
    {
        return _dbContext.Authors.FirstOrDefault(a => a.Id == authorId);
    }

    public Author GetAuthorByName(string authorName)
    {
        return _dbContext.Authors.FirstOrDefault(a => a.UserName == authorName);
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
            .FirstOrDefault(c => c.Author.UserName == newCheep.Author.UserName && c.TimeStamp == newCheep.TimeStamp);
        if (cheep != null)
        {
            cheep.Text = newCheep.Text;
            _dbContext.SaveChanges();
        }
    }
}