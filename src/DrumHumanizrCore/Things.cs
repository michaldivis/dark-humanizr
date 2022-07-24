using NAudio.Midi;

namespace DrumHumanizrCore;
internal class Things
{
    private const int TrackNumber = 0; //TODO don't just use the first track number, check if there are more and do something about it

    public List<int> GetDistinctNoteNumbers(MidiFile midiFile)
    {
        var noteEvents = midiFile.Events[TrackNumber]
            .Where(a => a is NoteEvent)
            .Cast<NoteEvent>()
            .ToList();

        var distinctNotes = noteEvents
            .Select(a => a.NoteNumber)
            .Distinct()
            .OrderBy(a => a)
            .ToList();

        return distinctNotes;
    }
}
