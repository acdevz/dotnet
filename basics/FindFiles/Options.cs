using CommandLine;

namespace FindFiles;

public record Options
{
    [Value(0, Required = true, HelpText = "Root folder")]
    public string RootFolder { get; set; } = ".";

    [Value(1, Required = false, HelpText = "Filename filter")]
    public string Filter { get; set; } = "*";

    [Option('r', "recurse", HelpText = "Search subfolders recursively")]
    public bool Recursive { get; set; } = false;

    [Option('b', "bare", HelpText = "Show files with fullpath")]
    public bool Bare { get; set; } = false;
}