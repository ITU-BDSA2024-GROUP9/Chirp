using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Core.Interfaces
{
    public interface IPost
    {
        public string author { get; }
        public string message { get; }
        public long timestamp { get; }
    }
}