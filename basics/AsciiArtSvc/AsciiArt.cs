using System.Reflection;
using Figgle;
using Figgle.Fonts;

namespace AsciiArtSvc;

public static class AsciiArt
{
    public static string? Write(string text, string? fontName = null)
    {
        FiggleFont? font = null;
        if (!string.IsNullOrWhiteSpace(fontName))
        {
            font = typeof(FiggleFonts)
            .GetProperty(fontName, BindingFlags.Static | BindingFlags.Public)?
            .GetValue(null) as FiggleFont;
        }
        else font = FiggleFonts.Standard;
        return font?.Render(text);
    }

    public static Lazy<IEnumerable<(string Name, FiggleFont? Font)>> AllFonts()
    {
        return new(() =>
        typeof(FiggleFonts)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
        .Select(p => (p.Name, p.GetValue(null) as FiggleFont)));
    }
}