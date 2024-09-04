using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Core.Classes
{
    public record Cheep(string Author, string Message, long Timestamp);
}