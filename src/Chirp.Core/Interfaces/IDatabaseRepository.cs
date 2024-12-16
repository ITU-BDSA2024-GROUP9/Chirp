using System.Globalization;
using Chirp.Core.Interfaces;




namespace SimpleDB.Services
{
    public interface IDatabaseRepository<T> where T : IPost
    {
        public Task<List<T>> Read(int? count = null);

        public Task Store(T record);

        public Task Delete(T record);

        public Task ArrangeTestDatabase();

        public Task ArrangeTMPDatabase();

    }
}