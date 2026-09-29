using GetJokes;
using CommandLine;
using CommandLine.Text;
using System.Text.Json;

var results = Parser.Default.ParseArguments<Options>(args)
    .WithParsed<Options>(o =>
    {
        var inFile = new FileInfo(o.InputFile!);
        if (!inFile.Exists)
        {
            Console.Error.WriteLine($"{inFile.FullName} does not exist");
            return;
        }
        using var inStream = inFile.OpenRead();

        FileInfo? outFile = null;
        if (o.OutputFile != null)
        {
            outFile = new FileInfo(o.OutputFile);
            if (outFile.Exists)
                outFile.Delete();
        }
        using var outStream = outFile != null ? outFile.OpenWrite() : Console.OpenStandardOutput();
        FindWithJsonDom(inStream, outStream, o.Category);
        FindWithSerialization(inStream, outStream, o.Category);
    });

results.WithNotParsed(_ => WriteLine(HelpText.RenderUsageText(results)));

static void FindWithJsonDom(Stream inStream, Stream outStream, string category)
{
    // var writer = new StreamWriter(outStream, Encoding.UTF8, leaveOpen: true); // why leave it open?
    using var writer = new Utf8JsonWriter(outStream, new JsonWriterOptions() { Indented = true });
    writer.WriteStartArray();

    using var jsonDoc = JsonDocument.Parse(inStream);
    foreach (var jsonElement in jsonDoc.RootElement.EnumerateArray())
    {
        string? type = jsonElement.GetProperty("type").GetString();
        if (string.Equals(type, category, StringComparison.OrdinalIgnoreCase))
        {
            // writer.WriteLine($"* {jsonElement.GetProperty("setup").GetString()} {jsonElement.GetProperty("punchline").GetString()}");
            string? setup = jsonElement.GetProperty("setup").GetString();
            string? punchline = jsonElement.GetProperty("punchline").GetString();
            writer.WriteStartObject();
            writer.WriteString("setup", setup);
            writer.WriteString("punchline", punchline);
            writer.WriteEndObject();
        }
    }
    writer.WriteEndArray();
}

static void FindWithSerialization(Stream inStream, Stream outStream, string category)
{
    var jokes = JsonSerializer.Deserialize<Joke[]>(
        inStream,
        new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        }
    );

    JsonSerializer.Serialize(
        outStream,
        jokes?
            .Where(j => string.Equals(j.Type, category, StringComparison.OrdinalIgnoreCase))
            .Select(j => new
            {
                setup = j.Setup,
                punchline = j.Punchline
            })
            .ToArray(),
        new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        }
    );
}
