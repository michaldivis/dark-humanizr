using CommandLine;
using DarkHumanizrCore;

var result = Parser.Default.ParseArguments<HumanizerOptions>(args)
    .WithParsed(Run);

static void Run(HumanizerOptions options)
{
    var result = HumanizationHandler.Humanize(options);

    if (!result.IsSuccess)
    {
        Console.WriteLine("ERRORS:");
        foreach (var error in result.Errors)
        {
            Console.WriteLine(error);
        }
        return;
    }

    Console.WriteLine("Done!");
}