﻿using Chirp.Core.Classes;
using Chirp.Core.DTO;

namespace Chirp.Services.Interfaces
{
	public interface ICheepService
	{
		public List<CheepDTO> GetCheeps(int page);
		public bool IsFollowing(AuthorDTO followerAuthor, AuthorDTO followedAuthor);
		public void Follow(AuthorDTO followerAuthor, AuthorDTO followedAuthor);
		public void Unfollow(AuthorDTO followerAuthor, AuthorDTO followedAuthor);

		public List<AuthorDTO> getFollowedInCheeps(AuthorDTO follower);
		public List<CheepDTO> GetCheepsFromAuthors(List<AuthorDTO> followedAuthors, string currentUserID, int pageNumber);
		public List<CheepDTO> GetCheepsFromAuthorByID(string authorId, int page);
		public List<CheepDTO> GetCheepsFromAuthorByName(string authorName, int page);
		public int GetCheepCountByAuthors(List<AuthorDTO> followedAuthors, string currentUserId);
		public CheepDTO GetCheepByID(int cheepId);
		public AuthorDTO? GetAuthorByID(string authorId);
		public AuthorDTO? GetAuthorByName(string name);
		public AuthorDTO? GetAuthorByEmail(string email);
		public int CreateCheep(CheepDTO cheep);
		public int GetCheepCount();
		public int GetCheepCountByID(string authorId);
		public int GetCheepByName(string authorName);
		public void UpdateCheep(CheepDTO cheep, int cheepId);
		public void DeleteCheep(int cheepId);
		Author ToDomain(AuthorDTO author);
		List<CommentDTO> GetCommentsForCheep(int cheepId);
		int GetCommentCountForCheep(int cheepId);
		void AddComment(CommentDTO comment);
		void DeleteComment(int commentId);
	}
}