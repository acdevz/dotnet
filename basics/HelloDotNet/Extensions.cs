using System.Text;

namespace HelloDotNet;

public static class StringExtensions
{
    public static string EveryOtherCharacter(this string s)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < s.Length; i += 2)
        {
            sb.Append(s[i]);
        }
        return sb.ToString();
    }
}