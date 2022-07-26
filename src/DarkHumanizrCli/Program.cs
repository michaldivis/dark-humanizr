using CommandLine;
using DarkHumanizrCli;
using DarkHumanizrCore;

var result = Parser.Default.ParseArguments<HumanizerOptions>(args)
    .WithParsed(App.Run);