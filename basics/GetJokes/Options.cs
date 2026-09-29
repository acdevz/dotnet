using CommandLine;

namespace GetJokes;

public record Options
{
    [Value(0, Required = true, HelpText = "Input file")]
    public string? InputFile { get; set; }

    [Value(1, Required = false, HelpText = "Output file")]
    public string? OutputFile { get; set; }

    [Option('c', "category", HelpText = "Category")]
    public string Category { get; set; } = "Programming";
}