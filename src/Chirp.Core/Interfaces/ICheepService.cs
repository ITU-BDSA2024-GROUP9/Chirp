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
        public Author GetAuthor(int authorId);
    }
}