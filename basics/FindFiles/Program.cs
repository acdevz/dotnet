using FindFiles;
using CommandLine;
using CommandLine.Text;
using System.Security;

var results = Parser.Default.ParseArguments<Options>(args)
    .WithParsed<Options>(o =>
    {
        var rootFolder = new DirectoryInfo(o.RootFolder);
        RecursiveFind(rootFolder, o.Filter, o.Recursive, o.Bare, 0);
    });
results.WithNotParsed(_ => WriteLine(HelpText.RenderUsageText(results)));

/* 
 * folder.GetFiles() -- files
 * folder.GetDirectories() -- subfolder
 */
static void RecursiveFind(
    DirectoryInfo folder,
    string filter,
    bool recurse,
    bool bare,
    int level
)
{
    if (!folder.Exists)
    {
        WriteLine($"{folder.FullName} does not exist");
        return;
    }
    if (!bare)
    {
        if (level == 0) WriteLine($"{new string('-', level)}> {folder.Name}");
        else WriteLine($"|{new string('-', level)}> {folder.Name}");
    }
    try
    {
        foreach (var file in folder.EnumerateFiles(filter))
        {
            if (!bare) WriteLine($"|{new string('-', level + 1)}- {file.Name}");
            else WriteLine($"{folder.FullName}\\{file.Name}");
        }
    }
    catch (SecurityException) { return; }

    if (recurse)
    {
        foreach (var subFolder in folder.GetDirectories())
        {
            RecursiveFind(subFolder, filter, recurse, bare, level + 1);
        }
    }
}