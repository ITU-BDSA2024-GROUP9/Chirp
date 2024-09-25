using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Core.Interfaces
{
    public interface IDatabaseService<T> where T : IPost
    {
        public Task<IEnumerable<T>> ReadAsync(int count = 0);
    }
}
