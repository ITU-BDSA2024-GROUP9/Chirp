﻿using Chirp.Core.Classes;
using Chirp.Core.DTO;
using Chirp.Repositories.Interfaces;
using Chirp.Services.Interfaces;

namespace Chirp.Services
{
	public class CheepService : ICheepService
	{
		private readonly ICheepRepository _repository;


		public CheepService(ICheepRepository repository)
		{
			_repository = repository;
		}

		public List<CheepDTO> GetCheeps(int page)
		{
			return _repository.GetCheeps(page);
		}

		public List<CheepDTO> GetCheepsFromAuthorByID(string authorId, int page)
		{
			return _repository.GetCheepsFromAuthorByID(authorId, page);
		}

		public List<CheepDTO> GetCheepsFromAuthorByName(string authorName, int page)
		{
			return _repository.GetCheepsFromAuthorByName(authorName, page);
		}

		public int CreateCheep(CheepDTO newCheep)
		{
			return _repository.CreateCheep(newCheep);
		}

		public bool IsFollowing(AuthorDTO followerAuthor, AuthorDTO followedAuthor)
		{
			return _repository.IsFollowing(followerAuthor, followedAuthor);
		}

		public List<AuthorDTO> getFollowedInCheeps(AuthorDTO follower)
		{
			return _repository.getFollowedInCheeps(follower);
		}

		public List<CheepDTO> GetCheepsFromAuthors(List<AuthorDTO> followedAuthors, string currentUserID, int pageNumber)
		{
			return _repository.GetCheepsFromAuthors(followedAuthors, currentUserID, pageNumber);
		}


		public void Follow(AuthorDTO followerAuthor, AuthorDTO followedAuthor)
		{
			_repository.Follow(followerAuthor, followedAuthor);
		}

		public void Unfollow(AuthorDTO followerAuthor, AuthorDTO followedAuthor)
		{
			_repository.Unfollow(followerAuthor, followedAuthor);
		}


		public AuthorDTO? GetAuthorByID(string authorId)
		{
			return _repository.GetAuthorByID(authorId);
		}

		public AuthorDTO? GetAuthorByName(string authorName)
		{
			return _repository.GetAuthorByName(authorName);
		}

		public AuthorDTO? GetAuthorByEmail(string email)
		{
			return _repository.GetAuthorByEmail(email);
		}

		public int GetCheepCount()
		{
			return _repository.GetCheepCount();
		}

		public int GetCheepCountByID(string authorId)
		{
			return _repository.GetCheepCountByID(authorId);
		}

		public int GetCheepByName(string authorName)
		{
			return _repository.GetCheepCountByName(authorName);
		}

		public Author ToDomain(AuthorDTO author)
		{
			return _repository.ToDomain(author);
		}

		public int GetCheepCountByAuthors(List<AuthorDTO> followedAuthors, string currentUserId)
		{
			return _repository.GetCheepCountByAuthors(followedAuthors, currentUserId);
		}

		public void UpdateCheep(CheepDTO newCheep, int cheepID)
		{
			_repository.UpdateCheep(newCheep, cheepID);
		}

		public void DeleteCheep(int cheepId)
		{
			_repository.DeleteCheep(cheepId);
		}

		public CheepDTO GetCheepByID(int cheepId)
		{
			return _repository.GetCheepByID(cheepId);
		}

		public List<CommentDTO> GetCommentsForCheep(int cheepId)
		{
			return _repository.GetCommentsForCheep(cheepId);
		}

		public int GetCommentCountForCheep(int cheepId)
		{
			return _repository.GetCommentsForCheep(cheepId).Count;
		}

		public void AddComment(CommentDTO comment)
		{
			_repository.AddComment(comment);
		}

		public void DeleteComment(int commentId)
		{
			_repository.DeleteComment(commentId);
		}
	}
}