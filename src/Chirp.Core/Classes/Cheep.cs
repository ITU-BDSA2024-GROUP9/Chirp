using Chirp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Core.Classes
{
    public record Cheep(string author, string message, long timestamp) : IPost;
}