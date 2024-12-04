using Chirp.Core.Classes;

namespace Chirp.Core.Helpers;

public class AuthorMapper
{
    public static AuthorDTO toDTO(Author author)
    {
        return new AuthorDTO
        {
            Id = author.Id,
            UserName = author.UserName,
            Email = author.Email,
            Cheeps = author.Cheeps,
            Following = author.Following,
            Followers = author.Followers,
            Comments = author.Comments
        };
    }

    public static Author toDomain(AuthorDTO authorDTO, ChirpDBContext dbContext)
    {
        var existingAuthor = dbContext.Authors.Find(authorDTO.Id);
        if (existingAuthor != null)
        {
            return existingAuthor;
        }
        
        return new Author
        {
            Id = authorDTO.Id,
            UserName = authorDTO.UserName,
            Email = authorDTO.Email,
            Cheeps = authorDTO.Cheeps,
            Following = authorDTO.Following,
            Followers = authorDTO.Followers,
            Comments = authorDTO.Comments
        };
    }
}