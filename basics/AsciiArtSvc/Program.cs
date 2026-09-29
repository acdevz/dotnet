using Figgle.Fonts;
using AsciiArtSvc;
using Figgle;
using BarcodeStandard;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", (int? take, int? skip, FiggleTextDirection? dir, string? order) =>
    {
        var query = AsciiArt.AllFonts().Value
            .Where(nt => dir == null || nt.Font?.Direction == dir)
            .Select(nt => nt.Name);
        if (string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase)) query = query.OrderByDescending(t => t);
        else query = query.OrderBy(t => t);
        return query
        .Skip(skip ?? 0)
        .Take(take ?? 10);
    }
);

app.MapGet("/{text}", (string text, string? font) =>
    {
        string? asciiArt = AsciiArt.Write(text, font);
        return asciiArt != null
            ? Results.Content(asciiArt)
            : Results.NotFound(new ErrorResponse("Uh oh! Font not found."));
    }
);

app.MapGet("/barcode/{text}", (string text, string? font) =>
    {
        string? asciiArt = AsciiArt.Write(text, font);
        if (asciiArt == null)
            return Results.NotFound(new ErrorResponse("Uh oh! Font not found."));
        var barcode = new Barcode();
        barcode.Encode(BarcodeStandard.Type.Code128, asciiArt, 10_000, 10_000);
        return Results.File(barcode.EncodedImageBytes, "image/jpeg", $"{DateTime.Now}_{font ?? "Standard"}");
    }
);


app.Run();
