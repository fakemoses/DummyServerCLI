using System;
using System.Collections.Generic;

public static class ArgumentHelper
{
    public static Dictionary<string, string> ParseArguments(string[] args)
    {
        var argDict = new Dictionary<string, string>();
        for (int i = 0; i < args.Length; i += 2)
        {
            if (i + 1 < args.Length)
            {
                argDict[args[i]] = args[i + 1];
            }
            else
            {
                Console.WriteLine($"Argument {args[i]} is missing a value.");
                ShowHelp();
                Environment.Exit(1);
            }
        }
        return argDict;
    }

    public static void ShowHelp()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  -c [tcp|serial]  : Connection type (default: tcp)");
        Console.WriteLine("  -ip [IP Address] : IP address (default: 127.0.0.1)");
        Console.WriteLine("  -p [Port]        : Port number (default: 6000)");
        Console.WriteLine("  -f [FilePath]    : Path to the file with commands (optional)");
        Console.WriteLine("  -h               : Display help message");
        Environment.Exit(0);
    }
}
