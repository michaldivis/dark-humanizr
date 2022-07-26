using CommandLine;

namespace DarkHumanizrCore;
[Verb("humanize", isDefault: true, HelpText = "Humanize a drum MIDI file")]
public class HumanizerOptions
{
    [Option(longName: "source", shortName: 's', Required = true, HelpText = "Source drum MIDI file")]
    public string SourceFilePath { get; init; } = null!;
    [Option(longName: "target", shortName: 't', Required = true, HelpText = "Target drum MIDI file, will create a new file or override an existing one")]
    public string TargetFilePath { get; init; } = null!;
    [Option(longName: "bpm", shortName: 'b', Required = false, Default = null, HelpText = "Static BPM to be used instead of reading tempo from MIDI tempo changes")]
    public int? Bpm { get; init; }
}
