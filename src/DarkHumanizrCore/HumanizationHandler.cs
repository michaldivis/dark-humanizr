using FluentResults;
using NAudio.Midi;

namespace DarkHumanizrCore;

public static class HumanizationHandler
{
    public static Result Humanize(HumanizerOptions options)
    {
        var randomizer = new Randomizer(new Random());

        var mfResult = TryLoadMidiFile(options.SourceFilePath);

        if (!mfResult.IsSuccess)
        {
            return Result.Fail(mfResult.Errors);
        }

        var mf = mfResult.Value;

        var timeWardenResult = options.Bpm is null
            ? TimeWarden.TryCreate(mf)
            : TimeWarden.CreateWithStaticTempo(mf.DeltaTicksPerQuarterNote, (int)options.Bpm);

        if (!timeWardenResult.IsSuccess)
        {
            return Result.Fail(timeWardenResult.Errors);
        }

        var humanizer = new Humanizer(mf, randomizer, timeWardenResult.Value);
        humanizer.Humanize();

        try
        {
            MidiFile.Export(options.TargetFilePath, mf.Events);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"MIDI file export failed: {options.TargetFilePath}").CausedBy(ex));
        }
    }

    private static Result<MidiFile> TryLoadMidiFile(string filePath)
    {
        try
        {
            return new MidiFile(filePath, false);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Failed to load MIDI file: {filePath}").CausedBy(ex));
        }
    }
}
