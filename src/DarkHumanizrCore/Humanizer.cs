using NAudio.Midi;

namespace DarkHumanizrCore;

internal class Humanizer
{
    private readonly Randomizer _randomizer;

    private const int _velocityChangeLimit = 10;

    public Humanizer(Randomizer randomizer)
    {
        _randomizer = randomizer;
    }

    public void HumanizeMidiFile(MidiFile mf, List<DrumSettings> settings)
    {
        for (int i = 0; i < mf.Tracks; i++)
        {
            HumanizeEvents(mf.Events[i], settings);
        }
    }

    private void HumanizeEvents(IList<MidiEvent> evnts, List<DrumSettings> settings)
    {
        var perDrumEvnts = evnts
            .Where(a => a is NoteOnEvent)
            .Cast<NoteOnEvent>()
            .OrderBy(a => a.AbsoluteTime)
            .GroupBy(a => a.NoteNumber);

        foreach (var drumEvnts in perDrumEvnts)
        {
            var noteNumber = drumEvnts.First().NoteNumber;
            var setting = FindSettingsForNoteNumber(settings, noteNumber);
            HumanizeDrumHits(setting, drumEvnts.ToList());
        }
    }

    private DrumSettings FindSettingsForNoteNumber(List<DrumSettings> settings, int noteNumber)
    {
        var setting = settings.FirstOrDefault(a => a.MidiNumber == noteNumber);
        return setting ?? new();
    }

    private void HumanizeDrumHits(DrumSettings settings, IList<NoteOnEvent> evnts)
    {
        for (int i = 0; i < evnts.Count; i++)
        {
            var current = evnts[i];
            var previous = GetPrevious(evnts, i);
            var next = GetNext(evnts, i);

            current.Velocity = _randomizer.RandomizeVelocity(current.Velocity, _velocityChangeLimit, _velocityChangeLimit);
        }
    }

    private NoteOnEvent? GetPrevious(IList<NoteOnEvent> evnts, int currentIndex)
    {
        if (currentIndex <=0)
        {
            return null;
        }

        return evnts[currentIndex - 1];
    }

    private NoteOnEvent? GetNext(IList<NoteOnEvent> evnts, int currentIndex)
    {
        if (currentIndex >= evnts.Count - 1)
        {
            return null;
        }

        return evnts[currentIndex + 1];
    }
}
