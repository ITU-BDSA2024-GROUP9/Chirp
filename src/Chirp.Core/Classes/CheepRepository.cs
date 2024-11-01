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

    public int CreateCheep(CheepDTO newCheep)
    {
        if (newCheep.Text.Length > 160)
        {
            throw new ArgumentException("Cheep text cannot be longer than 160 characters.");
        }

        var foundAuthor = GetAuthor(newCheep.Author.Id);
        if (foundAuthor == null)
        {
            throw new ArgumentException("Author not found.");
        }
        else foundAuthor.Cheeps ??= new List<Cheep>();

        var cheep = new Cheep
        {
            Author = foundAuthor,
            AuthorId = foundAuthor.Id,
            Text = newCheep.Text,
            TimeStamp = DateTime.Now
        };
        
        foundAuthor.Cheeps.Add(cheep);
    
        _dbContext.SaveChanges();
        return cheep.CheepId;
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
    
    public List<CheepDTO> ReadCheepsByID(string authorID)
    {
        var cheeps = _dbContext.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Id == authorID)
            .Select(c => new CheepDTO
            {
                Text = c.Text,
                TimeStamp = c.TimeStamp,
                Author = c.Author
            })
            .ToList();
        return cheeps;
    }



    public Author? GetAuthor(string authorId)
    {
        return _dbContext.Authors
            .Include(a => a.Cheeps) // Eager loading
            .FirstOrDefault(a => a.Id == authorId);
    }

    public Author? GetAuthorByName(string authorName)
    {
        return _dbContext.Authors
            .Include(a => a.Cheeps) // Eager loading
            .FirstOrDefault(a => a.UserName == authorName);
    }

    public Author? GetAuthorByEmail(string email)
    {
        return _dbContext.Authors
            .Include(a => a.Cheeps) // Eager loading
            .FirstOrDefault(a => a.Email == email);
    }

    public void CreateAuthor(Author newAuthor)
    {
        _dbContext.Authors.Add(newAuthor);
        _dbContext.SaveChanges();
    }

    public void UpdateCheep(CheepDTO newCheep, int cheepID)
    {
        var cheep = _dbContext.Cheeps.Find(cheepID);
        if (cheep != null)
        {
            cheep.Text = newCheep.Text;
            _dbContext.SaveChanges();
        }
    }
}