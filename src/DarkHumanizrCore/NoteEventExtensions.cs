using DarkMusicConcepts.Units;
using NAudio.Midi;

namespace DarkHumanizrCore;
internal static class NoteEventExtensions
{
    public static void DecreaseVelocity(this NoteEvent evnt, int percentage)
    {
        evnt.Velocity = DecreaseVelocity(evnt.Velocity, percentage);
    }

    private static int DecreaseVelocity(int velocity, int percentage)
    {
        var newVelocity = velocity - (velocity * percentage / 100);
        var safeVelocity = Math.Clamp(newVelocity, MidiVelocity.Min, MidiVelocity.Max);
        return safeVelocity;
    }
}
