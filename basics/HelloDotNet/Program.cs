using HelloDotNet;
using CommandLine;

// Console.WriteLine(StringExtensions.EveryOtherCharacter("Aman Chandra"));
// Console.WriteLine("Aman Chandra".EveryOtherCharacter()); // syntactic sugar

Parser.Default.ParseArguments<Options>(args)
    .WithParsed<Options>(o =>
        {
            if (o.List) AsciiArt.List();
            else AsciiArt.Write(o);
        }
    )
    .WithNotParsed(_ => Console.WriteLine("Usage: HelloDotNet <text> --font Big"));


