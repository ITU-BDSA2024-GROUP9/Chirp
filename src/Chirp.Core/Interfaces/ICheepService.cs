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
        public List<CheepDTO> GetCheepsFromAuthor(int authorId);
        public List<CheepDTO> GetCheepsFromAuthor(string authorName);
        public Author GetAuthor(int authorId);
        public Author GetAuthor(string name);
        public Author GetAuthorByEmail(string email);
    }
}