using System.Reflection;
using Figgle;
using Figgle.Fonts;

namespace HelloDotNet;

public static class AsciiArt
{
    public static void Write(Options o)
    {
        FiggleFont? font = null;
        if (!string.IsNullOrWhiteSpace(o.Font))
        {
            font = typeof(FiggleFonts)
            .GetProperty(o.Font, BindingFlags.Static | BindingFlags.Public)?
            .GetValue(null) as FiggleFont;
            if (font == null)
            {
                Console.WriteLine($"Could not find font '{o.Font}'");
            }
        }
        font ??= FiggleFonts.Standard;
        if (o?.Text != null)
        {
            var it = o.Text.GetEnumerator();
            while (it.MoveNext())
            {
                Console.WriteLine(font.Render(it.Current));
            }
            Console.WriteLine("Brought to you by " + typeof(AsciiArt).FullName);
        }
    }

    public static void List()
    {
        var list = typeof(FiggleFonts)
            .GetProperties()
            .ToList<PropertyInfo>();
        list.ForEach(pi => Console.WriteLine($"{pi.Name}"));
    }
}