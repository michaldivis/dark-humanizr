using CommandLine;
using DarkHumanizrCli;
using NAudio.Midi;

var result = Parser.Default.ParseArguments<HumanizerOptions>(args)
    .WithParsed(Run);

static void Run(HumanizerOptions options)
{
    var mf = new MidiFile(options.SourceFilePath, false);

    //TODO do stuff

    //MidiFile.Export(options.TargetFilePath, mf.Events);
}