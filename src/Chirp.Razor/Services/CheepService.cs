using Chirp.Core.Classes;
using Chirp.Core.Interfaces;
using System.ComponentModel.Design;

namespace Chirp.Razor.Services
{
    public class CheepService : ICheepService
    {
        private readonly ICheepRepository _repository;


        public CheepService(ICheepRepository repository)
        {
            _repository = repository;
        }
        public List<CheepDTO> GetCheeps()
        {
            return _repository.ReadCheeps();

        }

        public List<CheepDTO> GetCheepsFromAuthor(string authorId)
        {
            return _repository.ReadCheeps(authorId);
        }

        public List<CheepDTO> GetCheepsFromAuthorByName(string authorName)
        {
            return _repository.ReadCheeps(authorName);
        }

        public void CreateCheep(CheepDTO newCheep)
        {
            _repository.CreateCheep(newCheep);
        }

        public void UpdateCheep(CheepDTO newCheep, int cheepID)
        {
            _repository.UpdateCheep(newCheep, cheepID);
        }

        public Author GetAuthor(string authorId)
        {
            return _repository.GetAuthor(authorId);
        }

        public Author GetAuthorByName(string authorName)
        {
            return _repository.GetAuthor(authorName);
        }

        public Author GetAuthorByEmail(string email)
        {
            return _repository.GetAuthorByEmail(email);
        }
    }
}