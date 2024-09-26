using Chirp.Core.Classes;

namespace Chirp.Razor;

public class MathUtil
{
    static public int PageAmount(List<CheepViewModel> cheeps)
    {
        return (int) Math.Ceiling(1.0 * cheeps.Count / 32);
    }
}