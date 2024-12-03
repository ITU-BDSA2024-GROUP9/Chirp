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
            TimeStamp = DateTime.Now,
            Images = newCheep.Images
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
    
    public int GetCheepCountByAuthors(List<Author> followedAuthors, string currentUserId)
    {
        followedAuthors ??= new List<Author>();
        
        var authorIds = followedAuthors.Select(a => a.Id).ToList();
        if (!authorIds.Contains(currentUserId))
        {
            authorIds.Add(currentUserId);
        }

        // Return the count of cheeps where the author ID is in the list of author IDs
        return _dbContext.Cheeps
            .Include(c => c.Author) 
            .Count(c => authorIds.Contains(c.Author.Id));
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
                Author = c.Author,
                Images = c.Images
            })
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32)
            .ToList();
        return cheeps;
    }

    public List<Author> getFollowedInCheeps(Author follower)
    {
        List<Author> authors = _dbContext.Follows
            .Where(f => f.Follower == follower)
            .Select(f => f.Followed)
            .Where(a => a != null)
            .Distinct()
            .ToList()!;
        
        
        return authors;
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
                Author = c.Author,
                Images = c.Images
            })
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32)
            .ToList();
        return cheeps;
    }

    public List<CheepDTO> GetCheepsFromAuthors(List<Author> followedAuthors, string currentUserID, int pageNumber)
    {
        var authorIds = followedAuthors.Select(a => a.Id).ToList();
        authorIds.Add(currentUserID);
        
        var cheeps = _dbContext.Cheeps
            .Include(c => c.Author)
            .Where(c => authorIds.Contains(c.Author.Id))
            .Select(c => new CheepDTO
            {
                CheepId = c.CheepId,
                Text = c.Text,
                TimeStamp = c.TimeStamp,
                Author = c.Author,
                Images = c.Images
            })
            .OrderByDescending(c => c.TimeStamp)
            .Skip((pageNumber - 1) * 32)
            .Take(32)
            .ToList();
        return cheeps;
    }
        
    public bool IsFollowing(Author followerAuthor, Author followedAuthor){
        return followerAuthor.Following.Any(f => f.FollowedId == followedAuthor.Id);    
    }

    public void Follow(Author followerAuthor, Author followedAuthor)
    {
        var followEntry = new Follow() { Followed = followedAuthor, Follower = followerAuthor, FollowedId = followedAuthor.Id, FollowerId = followerAuthor.Id};
        _dbContext.Follows.Add(followEntry);
        _dbContext.SaveChanges();
    }
    public void Unfollow(Author followerAuthor, Author followedAuthor)
    {
        var followEntry = new Follow() { Followed = followedAuthor, Follower = followerAuthor, FollowedId = followedAuthor.Id, FollowerId = followerAuthor.Id};
        _dbContext.Follows.Remove(followEntry);
        _dbContext.SaveChanges();
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
                Author = c.Author,
                Images = c.Images
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
            Author = cheep.Author,
            Images = cheep.Images
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

    public List<CommentDTO> GetCommentsForCheep(int cheepId)
    {
        return _dbContext.Comments
            .Include(c => c.Author)
            .Where(c => c.CheepId == cheepId)
            .Select(c => new CommentDTO
            {
                CommentId = c.CommentId,
                Text = c.Text,
                TimeStamp = c.TimeStamp,
                Author = c.Author,
                CheepId = c.CheepId
            })
            .OrderBy(c => c.TimeStamp)
            .ToList();
    }

    public void AddComment(CommentDTO comment)
    {
        var cheep = _dbContext.Cheeps.Find(comment.CheepId);
        if (cheep == null)
        {
            throw new ArgumentException("Cheep not found.");
        }

        var newComment = new Comment
        {
            Text = comment.Text,
            TimeStamp = DateTime.Now,
            Author = comment.Author,
            AuthorId = comment.Author.Id,
            Cheep = cheep,
            CheepId = cheep.CheepId
        };

        _dbContext.Comments.Add(newComment);
        _dbContext.SaveChanges();
    }

    public void DeleteComment(int commentId)
    {
        var comment = _dbContext.Comments.Find(commentId);
        if (comment != null)
        {
            _dbContext.Comments.Remove(comment);
            _dbContext.SaveChanges();
        }
    }
}