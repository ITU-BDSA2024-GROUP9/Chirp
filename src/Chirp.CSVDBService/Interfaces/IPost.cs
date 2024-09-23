using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.CSVDBService.Interfaces
{
    public interface IPost
    {
        public string Author { get; }
        public string Message { get; }
        public long Timestamp { get; }
    }
}
