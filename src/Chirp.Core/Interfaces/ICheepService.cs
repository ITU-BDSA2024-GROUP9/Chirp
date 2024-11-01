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
        public List<CheepDTO> GetCheeps();
        public List<CheepDTO> GetCheepsFromAuthorByID(string authorId);
        public List<CheepDTO> GetCheepsFromAuthorByName(string authorName);
        public Author? GetAuthorByID(string authorId);
        public Author? GetAuthorByName(string name);
        public Author? GetAuthorByEmail(string email);
        public int CreateCheep(CheepDTO cheep);
    }
}