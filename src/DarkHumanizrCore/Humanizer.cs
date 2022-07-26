using NAudio.Midi;

namespace DarkHumanizrCore;

internal class Humanizer
{
    private readonly Randomizer _randomizer;
    private readonly MidiFile _mf;
    private readonly TimeWarden _timeWarden;

    private const int _velocityStepPercentage = 10;

    public Humanizer(MidiFile mf, Randomizer randomizer, TimeWarden timeWarden)
    {
        _mf = mf;
        _randomizer = randomizer;
        _timeWarden = timeWarden;        
    }

    public void Humanize()
    {
        for (int i = 0; i < _mf.Tracks; i++)
        {
            HumanizeEvents(_mf.Events[i]);
        }
    }

    private void HumanizeEvents(IList<MidiEvent> evnts)
    {
        var perDrumEvnts = evnts
            .Where(a => a is NoteOnEvent)
            .Cast<NoteOnEvent>()
            .OrderBy(a => a.AbsoluteTime)
            .GroupBy(a => a.NoteNumber);

        foreach (var drumEvnts in perDrumEvnts)
        {
            HumanizeDrumHits(drumEvnts.ToList());
        }
    }

    private void HumanizeDrumHits(IList<NoteOnEvent> evnts)
    {
        for (int i = 0; i < evnts.Count; i++)
        {
            var current = evnts[i];
            var previous = GetPrevious(evnts, i);
            var next = GetNext(evnts, i);

            EnsureAllowedVelocity(current, previous, next);            

            var hasBeenHumanizedBasedOnNext = HumanizeNeighboringHits(current, next);
            if (!hasBeenHumanizedBasedOnNext)
            {
                HumanizeNeighboringHits(current, previous);
            }

            RandomizeVelocity(current);
        }
    }

    private bool HumanizeNeighboringHits(NoteOnEvent current, NoteOnEvent? other)
    {
        if(other is null)
        {
            return false;
        }

        var tooCloseToOther = _timeWarden.AreNotesTooCloseToBePlayedHard(current, other);
        if (!tooCloseToOther)
        {
            return false;
        }

        var isCurrentMoreImportant = _timeWarden.IsCurrentMoreImportant(current, other);
        if (isCurrentMoreImportant)
        {
            return false;
        }

        current.DecreaseVelocity(_velocityStepPercentage);
        return true;
    }

    private static NoteOnEvent? GetPrevious(IList<NoteOnEvent> evnts, int currentIndex)
    {
        if (currentIndex <= 0)
        {
            return null;
        }

        return evnts[currentIndex - 1];
    }

    private static NoteOnEvent? GetNext(IList<NoteOnEvent> evnts, int currentIndex)
    {
        if (currentIndex >= evnts.Count - 1)
        {
            return null;
        }

        return evnts[currentIndex + 1];
    }

    private int GetMaxAllowedVelocity(long distanceFromOtherHits)
    {
        if(distanceFromOtherHits > _mf.DeltaTicksPerQuarterNote / 2)
        {
            //8th notes or slower
            return MidiVelocity.Max;
        }

        if (distanceFromOtherHits > _mf.DeltaTicksPerQuarterNote / 4)
        {
            //16th notes
            return 110;
        }

        if (distanceFromOtherHits > _mf.DeltaTicksPerQuarterNote / 8)
        {
            //32nd notes
            return 100;
        }

        return 90;
    }

    private void EnsureAllowedVelocity(NoteOnEvent current, NoteOnEvent? previous, NoteOnEvent? next)
    {
        var maxAllowedVelocity = MidiVelocity.Max;
        long distanceFromPrevious = 0;
        long distanceFromNext;

        if (previous is not null)
        {
            distanceFromPrevious = current.AbsoluteTime - previous.AbsoluteTime;
            maxAllowedVelocity = GetMaxAllowedVelocity(distanceFromPrevious);
        }

        if (next is not null)
        {
            distanceFromNext = current.AbsoluteTime - next.AbsoluteTime;
            if(distanceFromNext > distanceFromPrevious)
            {
                maxAllowedVelocity = GetMaxAllowedVelocity(distanceFromNext);
            }
        }

        if(current.Velocity > maxAllowedVelocity)
        {
            current.Velocity = maxAllowedVelocity;
        }
    }

    private void RandomizeVelocity(NoteOnEvent evnt)
    {
        evnt.Velocity = _randomizer.RandomizeVelocity(evnt.Velocity, 2, 2);
    }
}
