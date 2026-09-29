using CommandLine;

namespace HelloDotNet;

public record Options
{
    [Value(0)]
    public IEnumerable<string>? Text { get; init; }

    [Option('f', "font")]
    public string? Font { get; init; }

    [Option('l', "list", Required = false)]
    public bool List { get; init; }
}