using Chirp.Core.Interfaces;

namespace Chirp.Razor.Services
{
    public class MockCheepService<T> : IDatabaseService<T> where T : IPost
    {
        public Task<IEnumerable<T>> ReadAsync(int count = 0)
        {
            throw new NotImplementedException();
        }
    }
}
