﻿using Chirp.Core.Classes;
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

        public List<CheepDTO> GetCheepsFromAuthor(int authorId)
        {
            return _repository.ReadCheeps(authorId);
        }

        public void CreateCheep(CheepDTO newCheep)
        {
            _repository.CreateCheep(newCheep);
        }

        public void UpdateCheep(CheepDTO newCheep)
        {
            _repository.UpdateCheep(newCheep);
        }

        public Author GetAuthor(int authorId)
        {
            return _repository.GetAuthor(authorId);
        }
    }
}