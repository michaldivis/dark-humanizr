using DarkHumanizrCore;
using FluentResults;
using System.Drawing;
using System.Reflection;
using System.Text.Json;
using Console = Colorful.Console;

namespace DarkHumanizrCli;
internal static class App
{
    public static void Run(HumanizerOptions options)
    {
        PrintHeader();

        Console.WriteLine();
        Console.WriteLine();

        PrintOptions(options);

        var result = HumanizationHandler.Humanize(options);

        Console.WriteLine();

        PrintResult(result);
    }

    private static void PrintResult(Result result)
    {
        if (!result.IsSuccess)
        {
            Console.WriteLine("On or more errors occured:", Color.Red);
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error, Color.Gray);
            }
            return;
        }

        Console.WriteLine("Success!", Color.Green);
    }

    private static void PrintOptions(HumanizerOptions options)
    {
        var optionsJson = JsonSerializer.Serialize(options, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Console.WriteLine("Current settings:");
        Console.WriteLine(optionsJson, Color.Gray);
    }

    private static void PrintHeader()
    {
        Console.WriteAscii("DARK HUMANIZR", Color.SkyBlue);
        var version = GetVersion();
        Console.Write("Version: ", Color.Gray);
        Console.Write(version, Color.SkyBlue);        
    }

    private static string GetVersion()
    {
        return Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";
    }
}
