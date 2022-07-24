using CommandLine;

namespace DarkHumanizrCli;
[Verb("humanize", isDefault: true, HelpText = "Humanize a drum MIDI file")]
internal class HumanizerOptions
{
    [Option(longName: "source", shortName: 's', Required = true, HelpText = "Source drum MIDI file")]
    public string? SourceFilePath { get; init; }
    [Option(longName: "target", shortName: 't', Required = true, HelpText = "Target drum MIDI file, will create a new file or override an existing one")]
    public string? TargetFilePath { get; init; }
    [Option(longName: "velocity", shortName: 'v', Required = false, Default = false, HelpText = "Indicates whether velocity should be humanized")]
    public bool ShouldHumanizeVelocity { get; init; }
    [Option(longName: "timing", shortName: 'm', Required = false, Default = false, HelpText = "Indicates whether timing should be humanized")]
    public bool ShouldHumanizeTiming { get; init; }
}
