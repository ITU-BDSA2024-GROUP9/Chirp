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

        public List<CheepDTO> GetCheepsFromAuthorByID(string authorId)
        {
            return _repository.ReadCheepsByID(authorId);
        }

        public List<CheepDTO> GetCheepsFromAuthorByName(string authorName)
        {
            return _repository.ReadCheepsByName(authorName);
        }

        public int CreateCheep(CheepDTO newCheep)
        {
            return _repository.CreateCheep(newCheep);
        }

        public void UpdateCheep(CheepDTO newCheep, int cheepID)
        {
            _repository.UpdateCheep(newCheep, cheepID);
        }

        public Author? GetAuthorByID(string authorId)
        {
            return _repository.GetAuthorByID(authorId);
        }

        public Author? GetAuthorByName(string authorName)
        {
            return _repository.GetAuthorByName(authorName);
        }

        public Author? GetAuthorByEmail(string email)
        {
            return _repository.GetAuthorByEmail(email);
        }
    }
}