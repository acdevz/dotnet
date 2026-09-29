using FindText;
using CommandLine;
using CommandLine.Text;
using System.Text.RegularExpressions;

var results = Parser.Default.ParseArguments<Options>(args)
    .WithParsed<Options>(o =>
    {
        if (o.FileName != null)
        {
            var file = new FileInfo(o.FileName);
            SearchFile(file, o.Text!, o.Regex); // ! tells compiler, it won't be null here, chill! 
        }
        else
        {
            string? fileName;
            while (!string.IsNullOrWhiteSpace(fileName = ReadLine()))
            {
                var file = new FileInfo(fileName);
                SearchFile(file, o.Text!, o.Regex);
            }
        }
    });

results.WithNotParsed(_ => WriteLine(HelpText.RenderUsageText(results)));

static void SearchFile(
    FileInfo file,
    string text,
    bool isRegex
)
{
    if (!file.Exists)
    {
        Console.Error.WriteLine($"{file.Name} does not exist");
        return;
    }

    try // try with resources --> using statement in try block -- the dispose pattern
    {
        using var reader = file.OpenText();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (!isRegex)
            {
                if (line.Contains(text, StringComparison.OrdinalIgnoreCase)) WriteLine(line);
            }
            else
            {
                var regex = new Regex(text, RegexOptions.IgnoreCase);
                if (regex.IsMatch(line)) WriteLine(line);
            }
        }
    }
    catch (UnauthorizedAccessException)
    {
        Console.Error.WriteLine($"Unauthorized: {file.FullName}");
    }
    catch (IOException)
    {
        Console.Error.WriteLine($"IO Error: {file.FullName}");
    }
}