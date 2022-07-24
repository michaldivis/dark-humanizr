using CommandLine;
using DarkHumanizrCore;

var result = Parser.Default.ParseArguments<HumanizerOptions>(args)
    .WithParsed(Humanizr.Humanize);