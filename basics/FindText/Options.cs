using CommandLine;

namespace FindText;

public record Options
{
    [Value(0, Required = true, HelpText = "Text to search for")]
    public string? Text { get; set; }

    [Value(1, Required = false, HelpText = "File to search")]
    public string? FileName { get; set; }

    [Option('x', "regex", Required = false, HelpText = "Search using regex")]
    public bool Regex { get; set; } = false;
}