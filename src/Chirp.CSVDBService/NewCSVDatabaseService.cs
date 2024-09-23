using Chirp.Core.Interfaces;
using Chirp.CSVDBService.Interfaces;

namespace Chirp.CSVDBService
{
    public class NewCSVDatabaseService<T> : IDatabaseRepository<T> where T : IPost
    {
        public async Task<List<T>> Read(int? count)
        {
            throw new NotImplementedException();
        }

        public async Task Store(T record)
        {
            throw new NotImplementedException();
        }
    }
}
