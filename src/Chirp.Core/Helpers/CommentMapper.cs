using Chirp.Core.Classes;

namespace Chirp.Core.Helpers;

public class CommentMapper
{
    public static CommentDTO toDTO (Comment comment)
    {
        return new CommentDTO
        {
            CommentId = comment.CommentId,
            Text = comment.Text,
            TimeStamp = comment.TimeStamp,
            Author = comment.Author,
            CheepId = comment.CheepId
        };
    }

    public static Comment toDomain(CommentDTO commentDTO, CheepDTO cheep)
    {
        return new Comment
        {
            CommentId = commentDTO.CommentId,
            Cheep = CheepMapper.toDomain(cheep),
            AuthorId = commentDTO.Author.Id,
            Text = commentDTO.Text,
            TimeStamp = commentDTO.TimeStamp,
            Author = commentDTO.Author,
            CheepId = commentDTO.CheepId
        };
    }
}