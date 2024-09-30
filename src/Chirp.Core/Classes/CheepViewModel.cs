using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Core.Classes
{
    public record CheepViewModel(string Author, string Message, string Timestamp);
}