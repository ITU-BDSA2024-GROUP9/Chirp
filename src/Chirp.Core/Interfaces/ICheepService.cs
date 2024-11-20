using Chirp.Core.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Core.Interfaces
{
    public interface ICheepService//<T> where T : IPost
    {     
        public List<CheepDTO> GetCheeps(int page);
        public bool IsFollowing(Author followerAuthor, Author followedAuthor);
        public void Follow(Author followerAuthor, Author followedAuthor);
        public void Unfollow(Author followerAuthor, Author followedAuthor);

        public List<Author> getFollowedInCheeps(Author follower);
        public List<CheepDTO> GetCheepsFromAuthors(List<Author> followedAuthors, string currentUserID, int pageNumber);
        public List<CheepDTO> GetCheepsFromAuthorByID(string authorId, int page);
        public List<CheepDTO> GetCheepsFromAuthorByName(string authorName, int page);
        public int GetCheepCountByAuthors(List<Author> followedAuthors, string currentUserId);
        public CheepDTO GetCheepByID(int cheepId);
        public Author? GetAuthorByID(string authorId);
        public Author? GetAuthorByName(string name);
        public Author? GetAuthorByEmail(string email);
        public int CreateCheep(CheepDTO cheep);
        public int GetCheepCount();
        public int GetCheepCountByID(string authorId);
        public int GetCheepByName(string authorName);
        public void UpdateCheep(CheepDTO cheep, int cheepId);
        public void DeleteCheep(int cheepId);
    }
}