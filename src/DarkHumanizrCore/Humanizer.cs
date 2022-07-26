using NAudio.Midi;

namespace DarkHumanizrCore;

internal class Humanizer
{
    private readonly Randomizer _randomizer;
    private readonly MidiFile _mf;
    private readonly TimeWarden _timeWarden;

    private const int _velocityStep = 15;

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

            HumanizeNeighboringHits(current, previous);
            HumanizeNeighboringHits(current, next);
        }
    }

    private void HumanizeNeighboringHits(NoteOnEvent current, NoteOnEvent? other)
    {
        if(other is null)
        {
            return;
        }

        var tooCloseToOther = _timeWarden.AreNotesTooCloseToBePlayedHard(current, other);
        if (!tooCloseToOther)
        {
            return;
        }

        var isCurrentMoreImportant = IsCurrentMoreImportant(current, other);
        if (isCurrentMoreImportant)
        {
            return;
        }

        current.DecreaseVelocity(_velocityStep);
    }

    private bool IsCurrentMoreImportant(NoteOnEvent current, NoteOnEvent other)
    {
        var isCurrentOnQuarterBeat = _timeWarden.IsOnQuarterBeat(current.AbsoluteTime);
        var isOtherOnQuarterBeat = _timeWarden.IsOnQuarterBeat(other.AbsoluteTime);

        if(isCurrentOnQuarterBeat && !isOtherOnQuarterBeat)
        {
            return true;
        }

        var isCurrentOnEightBeat = _timeWarden.IsOnEightBeat(current.AbsoluteTime);
        var isOtherOnEightBeat = _timeWarden.IsOnEightBeat(other.AbsoluteTime);

        if (isCurrentOnEightBeat && !isOtherOnEightBeat)
        {
            return true;
        }

        var isCurrentOnSixteenthBeat = _timeWarden.IsOnSixteenthBeat(current.AbsoluteTime);
        var isOtherOnSixteenthBeat = _timeWarden.IsOnSixteenthBeat(other.AbsoluteTime);

        if (isCurrentOnSixteenthBeat && !isOtherOnSixteenthBeat)
        {
            return true;
        }

        var isCurrentOnThirtySecondBeat = _timeWarden.IsOnThirtySecondBeat(current.AbsoluteTime);
        var isOtherOnThirtySecondBeat = _timeWarden.IsOnThirtySecondBeat(other.AbsoluteTime);

        if (isCurrentOnThirtySecondBeat && !isOtherOnThirtySecondBeat)
        {
            return true;
        }

        return false;
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
