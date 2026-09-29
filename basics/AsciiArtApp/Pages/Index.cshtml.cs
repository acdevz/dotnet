using System.Reflection;
using Figgle;
using Figgle.Fonts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsciiArtWebApp.Pages;

public class IndexModel : PageModel
{
    public string? RenderedText { get; set; }

    public bool ShowRenderedText =>
        !string.IsNullOrWhiteSpace(RenderedText);

    public IEnumerable<(string Name, FiggleFont? Font)> Fonts =>
        AsciiArt.AllFonts().Value;

    public void OnGet()
    {
    }

    public void OnPost(string text, string fontName)
    {
        RenderedText = AsciiArt.Write(
            text ?? "Hey!",
            fontName);
    }
}

public static class AsciiArt
{
    public static string? Write(
        string text,
        string? fontName = null)
    {
        FiggleFont? font = null;

        if (!string.IsNullOrWhiteSpace(fontName))
        {
            font = typeof(FiggleFonts)
                .GetProperty(
                    fontName,
                    BindingFlags.Static | BindingFlags.Public)?
                .GetValue(null) as FiggleFont;
        }
        else
        {
            font = FiggleFonts.Standard;
        }

        return font?.Render(text);
    }

    public static Lazy<IEnumerable<(string Name, FiggleFont? Font)>> AllFonts()
    {
        return new(() =>
            typeof(FiggleFonts)
                .GetProperties(
                    BindingFlags.Public | BindingFlags.Static)
                .Select(p => (
                    p.Name,
                    p.GetValue(null) as FiggleFont
                )));
    }
}
