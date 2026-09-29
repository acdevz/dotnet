using XkcdSearch;
using CommandLine;
using CommandLine.Text;

var results = await Parser.Default.ParseArguments<Options>(args)
    .WithParsedAsync<Options>(async o =>
    {
        var comic = await Comic.GetComicWithTitleAsync(o.TitleText!);
        if (comic == null)
        {
            WriteLine($"xkcd comic with title \"{o.TitleText}\" not found");
        }
        else
        {
            WriteLine($"xkcd \"{comic.Title}\" is number {comic.Number} published on {comic.Date}");
        }
    });

results.WithNotParsed(_ => WriteLine(HelpText.RenderUsageText(results)));
