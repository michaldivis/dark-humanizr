using FluentResults;
using NAudio.Midi;

namespace DarkHumanizrCore;

internal class TimeWarden
{
    private const int _microsecondsPerMinute = 60_000_000;

    record PointInMidiTime(TempoEvent TempoEvent, TimeSpan StartTime);

    private readonly List<PointInMidiTime> _points;
    private readonly int _deltaTicksPerQuarterNote;

    public static Result<TimeWarden> CreateWithStaticTempo(int deltaTicksPerQuarterNote, int bpm)
    {
        var tempoEvents = new List<TempoEvent>
        {
            new TempoEvent(GetMicrosecondsPerQuarterNote(bpm), 0)
        };

        var timeWarden = new TimeWarden(deltaTicksPerQuarterNote, tempoEvents);
        return Result.Ok(timeWarden);
    }

    public static Result<TimeWarden> TryCreate(MidiFile mf)
    {
        var tempoEvents = mf.Events
            .Where(a => a is TempoEvent)
            .Cast<TempoEvent>()
            .OrderBy(a => a.AbsoluteTime)
            .ToList();

        if(!tempoEvents.Any())
        {
            return Result.Fail("MIDI file doesn't contain any tempo events");
        }

        var timeWarden = new TimeWarden(mf.DeltaTicksPerQuarterNote, tempoEvents);
        return Result.Ok(timeWarden);
    }

    private TimeWarden(int deltaTicksPerQuarterNote, List<TempoEvent> tempoEvents)
    {
        _deltaTicksPerQuarterNote = deltaTicksPerQuarterNote;
        _points = CreatePoints(tempoEvents);
    }

    private List<PointInMidiTime> CreatePoints(List<TempoEvent> tempoChanges)
    {
        var points = new List<PointInMidiTime>();

        var alreadyCoveredAbsoluteTime = 0L;
        var previousStartTime = TimeSpan.Zero;

        foreach (var tempoEvent in tempoChanges)
        {
            var startTime = previousStartTime + CalculateTime(_deltaTicksPerQuarterNote, tempoEvent.MicrosecondsPerQuarterNote, tempoEvent.AbsoluteTime - alreadyCoveredAbsoluteTime);

            points.Add(new PointInMidiTime(tempoEvent, startTime));

            previousStartTime = startTime;
            alreadyCoveredAbsoluteTime = tempoEvent.AbsoluteTime;
        }

        return points;
    }

    private static TimeSpan CalculateTime(int ticksPerQuarter, int microsecondsPerQuarter, long ticks)
    {
        var microsecondsPerTick = (double)microsecondsPerQuarter / ticksPerQuarter;
        var secondsPerTick = (double)microsecondsPerTick / 1_000_000;
        var seconds = ticks * secondsPerTick;
        return TimeSpan.FromSeconds(seconds);
    }

    public bool AreNotesTooCloseToBePlayedHard(NoteEvent a, NoteEvent b)
    {
        var difference = Math.Abs(a.AbsoluteTime - b.AbsoluteTime);
        return difference < _deltaTicksPerQuarterNote / 2;
    }

    public bool IsCurrentMoreImportant(NoteEvent current, NoteEvent other)
    {
        var beatTime = _deltaTicksPerQuarterNote * 4; //whole notes

        while (true)
        {
            if(beatTime < _deltaTicksPerQuarterNote / 8)
            {
                //if less than 32nd notes, quit
                return false;
            }

            var isCurrentOnBeat = IsOnBeat(current.AbsoluteTime, beatTime);
            var isOtherOnBeat = IsOnBeat(other.AbsoluteTime, beatTime);

            if (isCurrentOnBeat && !isOtherOnBeat)
            {
                return true;
            }

            beatTime /= 2; //decrease beatTime
        }
    }

    private bool IsOnBeat(long absoluteTime, long beatTime)
    {
        var point = GetLastPointBeforeTime(absoluteTime);
        var relativeTimeWithinPoint = absoluteTime - point.TempoEvent.AbsoluteTime;
        var isOnAQuarterBeat = IsTimeOnBeat(relativeTimeWithinPoint, beatTime);
        return isOnAQuarterBeat;
    }

    private bool IsTimeOnBeat(long time, long beatTime)
    {
        var distanceFromBeat = time % beatTime;
        var isOnBeat = distanceFromBeat < 10;
        return isOnBeat;
    }

    private PointInMidiTime GetLastPointBeforeTime(long absoluteTime)
    {
        var point = _points.LastOrDefault(a => a.TempoEvent.AbsoluteTime < absoluteTime);

        if (point is null)
        {
            //TODO handle error
            throw new Exception("Event time is before any recorded tempo changes");
        }

        return point;
    }

    private static int GetMicrosecondsPerQuarterNote(int bpm)
    {
        return _microsecondsPerMinute / bpm;
    }
}
