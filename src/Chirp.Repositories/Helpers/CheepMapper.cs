using Chirp.Core.Classes;
using Chirp.Core.DTO;

namespace Chirp.Repositories.Helpers;

public class CheepMapper
{
	public static CheepDTO toDTO(Cheep cheep)
	{
		return new CheepDTO
		{
			CheepId = cheep.CheepId,
			Text = cheep.Text,
			TimeStamp = cheep.TimeStamp,
			Author = cheep.Author,
			Images = cheep.Images
		};
	}

	public static Cheep toDomain(CheepDTO cheepDTO)
	{
		return new Cheep
		{
			CheepId = cheepDTO.CheepId,
			Text = cheepDTO.Text,
			TimeStamp = cheepDTO.TimeStamp,
			Author = cheepDTO.Author,
			AuthorId = cheepDTO.Author.Id,
			Images = cheepDTO.Images
		};
	}
}