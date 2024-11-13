using System.Security.Cryptography;

namespace LaytonYSAClerk.WebTool.Services;

public static class Shuffler
{
    public static List<T> Shuffle<T>(this IList<T> input)
    {
        var list = input.ToList();
        using var provider = new RNGCryptoServiceProvider();
        var n = list.Count;
        while (n > 1)
        {
            var box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            var k = (box[0] % n);
            n--;
            (list[k], list[n]) = (list[n], list[k]);
        }
        return list;
    }
}