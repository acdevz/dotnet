using CmdArgsTemplate;
using CommandLine;
using CommandLine.Text;

var results = Parser.Default.ParseArguments<Options>(args)
    .WithParsed<Options>(o => { WriteLine(o.Text); });

results.WithNotParsed(_ => WriteLine(HelpText.RenderUsageText(results)));
