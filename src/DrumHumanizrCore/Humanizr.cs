using FluentResults;
using NAudio.Midi;
using System.Text.Json;

namespace DrumHumanizrCore;

public static class Humanizr
{
    public static void Humanize(HumanizerOptions options)
    {
        var settings = LoadSettings(options.SettingsFilePath);

        var mf = new MidiFile(options.SourceFilePath, false);

        var distinctNotes = MappingLoader.GetDistinctNotes(mf);

        //TODO humanize

        //MidiFile.Export(options.TargetFilePath, mf.Events);
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
            return Result.Fail(ex.ToString());
        }
    }
}
