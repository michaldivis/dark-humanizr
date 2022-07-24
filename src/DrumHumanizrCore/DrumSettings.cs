using DarkMusicConcepts;
using System.Text.Json.Serialization;

namespace DrumHumanizrCore;

public class DrumSettings
{
    public MidiNumber MidiNumber { get; init; } = MidiNumber.From(0);
    public MidiVelocity DefaultVelocity { get; init; } = MidiVelocity.From(0);
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DrumType DrumType { get; init; } = DrumType.Cymbal;
}
