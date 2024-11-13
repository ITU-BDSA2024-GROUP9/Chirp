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

        var foundAuthor = GetAuthorByID(newCheep.Author.Id);
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

    public int GetCheepCount()
    {
        return _dbContext.Cheeps.Count();
    }

    public int GetCheepCountByID(string authorId)
    {
        return _dbContext.Cheeps
            .Include(c => c.Author)
            .Count(c => c.Author.Id == authorId);
    }

    public int GetCheepCountByName(string authorName)
    {
        return _dbContext.Cheeps
            .Include(c => c.Author)
            .Count(c => EF.Functions.Collate(c.Author.UserName, "NOCASE") == authorName);
    }

    public List<CheepDTO> GetCheeps(int page)
    {
        var cheeps = _dbContext.Cheeps
            .Include(c => c.Author)
            .Select(c => new CheepDTO
            {
                CheepId = c.CheepId,
                Text = c.Text,
                TimeStamp = c.TimeStamp,
                Author = c.Author
            })
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32)
            .ToList();
        return cheeps;
    }

    public List<CheepDTO> GetCheepsFromFollowed(string authorId)
    {
        return null;
    }

    public List<CheepDTO> GetCheepsFromAuthorByName(string authorName, int page)
    {
        var cheeps = _dbContext.Cheeps
            .Include(c => c.Author)
            .Where(c => EF.Functions.Collate(c.Author.UserName, "NOCASE") == authorName)
            .Select(c => new CheepDTO
            {
                CheepId = c.CheepId,
                Text = c.Text,
                TimeStamp = c.TimeStamp,
                Author = c.Author
            })
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32)
            .ToList();
        return cheeps;
    }
        
    public bool IsFollowing(Author followerAuthor, Author followedAuthor){
        return followerAuthor.Following.Any(f => f.FollowedId == followedAuthor.Id);    
    }

    public List<CheepDTO> GetCheepsFromAuthorByID(string authorID, int page)
    {
        var cheeps = _dbContext.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Id == authorID)
            .Select(c => new CheepDTO
            {
                CheepId = c.CheepId,
                Text = c.Text,
                TimeStamp = c.TimeStamp,
                Author = c.Author
            })
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32)
            .ToList();
        return cheeps;
    }

    public CheepDTO GetCheepByID(int cheepID)
    {
        var cheep = _dbContext.Cheeps
            .Include(c => c.Author)
            .FirstOrDefault(c => c.CheepId == cheepID);
        if (cheep == null)
        {
            throw new ArgumentException("Cheep not found.");
        }

        return new CheepDTO
        {
            CheepId = cheep.CheepId,
            Text = cheep.Text,
            TimeStamp = cheep.TimeStamp,
            Author = cheep.Author
        };
    }

    public void DeleteCheep(int cheepID)
    {
        var cheep = _dbContext.Cheeps.Find(cheepID);
        if (cheep != null)
        {
            _dbContext.Cheeps.Remove(cheep);
            _dbContext.SaveChanges();
        }
    }



    public Author? GetAuthorByID(string authorId)
    {
        return _dbContext.Authors
            .Include(a => a.Cheeps) // Eager loading
            .FirstOrDefault(a => a.Id == authorId);
    }

    public Author? GetAuthorByName(string authorName)
    {
        return _dbContext.Authors
            .Include(a => a.Cheeps) // Eager loading
            .FirstOrDefault(a => EF.Functions.Collate(a.UserName, "NOCASE") == authorName);
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