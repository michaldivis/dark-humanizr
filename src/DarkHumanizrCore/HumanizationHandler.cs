using FluentResults;
using NAudio.Midi;
using System.Text.Json;

namespace DarkHumanizrCore;

public static class HumanizationHandler
{
    public static void Humanize(HumanizerOptions options)
    {
        var randomizer = new Randomizer(new Random());
        var humanizer = new Humanizer(randomizer);

        //var settingsResult = LoadSettings(options.SettingsFilePath);

        //if (!settingsResult.IsSuccess)
        //{
        //    return;
        //}

        var settings = new List<DrumSettings>(); //TODO read settings from a JSON file

        var mf = new MidiFile(options.SourceFilePath, false);

        humanizer.HumanizeMidiFile(mf, settings);

        MidiFile.Export(options.TargetFilePath, mf.Events);
    }

    private static Result<List<DrumSettings>> LoadSettings(string settingsFilePath)
    {
        try
        {
            var json = File.ReadAllText(settingsFilePath);
            var settings = JsonSerializer.Deserialize<List<DrumSettings>>(json);

            if(settings is null)
            {
                return Result.Fail("Failed to parse settings, settings is null");
            }

            return settings;
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to parse settings").CausedBy(ex));
        }
    }
}
