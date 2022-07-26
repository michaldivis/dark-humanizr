using FluentResults;
using NAudio.Midi;

namespace DarkHumanizrCore;

public static class HumanizationHandler
{
    public static Result Humanize(HumanizerOptions options)
    {
        var randomizer = new Randomizer(new Random());

        var mf = new MidiFile(options.SourceFilePath, false);

        var timeWardenResult = options.Bpm is null
            ? TimeWarden.TryCreate(mf)
            : TimeWarden.CreateWithStaticTempo(mf.DeltaTicksPerQuarterNote, (int)options.Bpm);

        if (!timeWardenResult.IsSuccess)
        {
            return Result.Fail(timeWardenResult.Errors);
        }

        var humanizer = new Humanizer(mf, randomizer, timeWardenResult.Value);
        humanizer.Humanize();

        MidiFile.Export(options.TargetFilePath, mf.Events);

        return Result.Ok();
    }
}
