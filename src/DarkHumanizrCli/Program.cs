using CommandLine;
using DrumHumanizrCore;

var result = Parser.Default.ParseArguments<HumanizerOptions>(args)
    .WithParsed(Humanizr.Humanize);