using DarkMusicConcepts;
using NAudio.Midi;

namespace DarkHumanizrCore;
public static class MappingLoader
{
    private const int TrackNumber = 0; //TODO don't just use the first track number, check if there are more and do something about it

    public static List<Note> GetDistinctNotes(MidiFile midiFile)
    {
        var noteEvents = midiFile.Events[TrackNumber]
            .Where(a => a is NoteEvent)
            .Cast<NoteEvent>()
            .ToList();

        var distinctNoteNumbers = noteEvents
            .Select(a => a.NoteNumber)
            .Distinct()
            .OrderBy(a => a);

        var distinctNotes = distinctNoteNumbers
            .Select(a => Note.FindByMidiNumber(a))
            .ToList();

        return distinctNotes;
    }
}
