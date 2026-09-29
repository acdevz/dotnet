using CommandLine;

namespace XkcdSearch;

public record Options
{
    [Value(0, Required = true, HelpText = "Comic Title Text")]
    public string? TitleText { get; set; }
}