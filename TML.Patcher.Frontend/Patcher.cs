﻿using System;
using System.IO;
using Consolation;
using Consolation.Common;
using Consolation.Common.Framework.OptionsSystem;
using TML.Files;
using TML.Patcher.Frontend.Common;
using TML.Patcher.Frontend.Common.Options;

namespace TML.Patcher.Frontend
{
    public sealed class Patcher : ConsoleWindow
    {
        public const string Line = "-----------------------------------------------------------------";

        public override ConsoleOptions DefaultOptions => Program.DefaultOptions;

        /// <summary>
        /// Writes text that will always show at the beginning, and should persist after clears.
        /// </summary>
        public override void WriteStaticText(bool withMessage)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;

            string[] whatCanThisDoLol =
            {
                "Unpack .tmod files",
                "Repack .tmod files",
                "Decompile stored assembles in .tmod files",
                "Patch and recompile assemblies"
            };

            string[] contributors =
            {
                "convicted tomatophile (Stevie) - Main developer",
                "Trivaxy - Original mod unpacking code",
                "Chik3r - Improved multithreading/task code",
                "Archanyhm - Help with Linux and Mac compatibility"
            };

            string[] versions =
            {
                $"TML.Patcher.Frontend v{typeof(TML.Patcher.Frontend.Program).Assembly.GetName().Version}",
                $"TML.Patcher.Backend v{typeof(TML.Patcher.Backend.Packing.UnpackRequest).Assembly.GetName().Version}",
                $"TML.Files v{typeof(TML.Files.Generic.Files.FileData).Assembly.GetName().Version}",
                $"Consolation v{typeof(Consolation.ConsoleAPI).Assembly.GetName().Version}"
            };

            Console.WriteLine();
            Console.WriteLine(" Welcome to TMLPatcher!");
            Console.WriteLine();

            Console.WriteLine(" Running:");
            foreach (string version in versions)
                Console.WriteLine($"  {version}");

            Console.WriteLine();

            Console.WriteLine(" This is a program that allows you to:");

            for (int i = 0; i < whatCanThisDoLol.Length; i++)
                Console.WriteLine($"  [{i + 1}] {whatCanThisDoLol[i]}");

            Console.WriteLine();

            Console.WriteLine(" Credits:");
            foreach (string contributor in contributors)
                Console.WriteLine($"  {contributor}");

            Console.WriteLine(Line);
            Console.WriteLine(" Loaded with configuration options:");
            Console.WriteLine($"  {nameof(Program.Configuration.ModsPath)}: {Program.Configuration.ModsPath}");
            Console.WriteLine($"  {nameof(Program.Configuration.ExtractPath)}: {Program.Configuration.ExtractPath}");
            Console.WriteLine($"  {nameof(Program.Configuration.DecompilePath)}: {Program.Configuration.DecompilePath}");
            Console.WriteLine($"  {nameof(Program.Configuration.ReferencesPath)}: {Program.Configuration.ReferencesPath}");
            Console.WriteLine($"  {nameof(Program.Configuration.Threads)}: {Program.Configuration.Threads}");

            Console.WriteLine(Line);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" Please note that if you are trying to decompile a mod,");
            Console.WriteLine(" you'll have to add all of the mod's required references to:");
            Console.WriteLine($" \"{Program.Configuration.ReferencesPath}\"");
            Console.WriteLine(" (i.e. tModLoader.exe, XNA DLLs, ...)");
            Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.WriteLine(Line);
            Console.WriteLine();

            if (!withMessage)
                Console.WriteLine();
        }

        public void CheckForUndefinedPath()
        {
            while (true)
            {
                if (!Program.Configuration.ModsPath.Equals("undefined") && Directory.Exists(Program.Configuration.ModsPath))
                    return;

                if (Environment.OSVersion.Platform == PlatformID.Unix && !Directory.Exists(Program.Configuration.ModsPath))
                    if (Directory.Exists(Environment.ExpandEnvironmentVariables(ConfigurationFile.LinuxDefault2)))
                    {
                        Program.Configuration.ModsPath = Environment.ExpandEnvironmentVariables(ConfigurationFile.LinuxDefault2);
                        ConfigurationFile.Save();
                        return;
                    }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($" {nameof(Program.Configuration.ModsPath)} is undefined or was not found!");
                Console.WriteLine(" Please enter the directory of your tModLoader Mods folder:");
                
                string modsPath = Console.ReadLine();

                if (Directory.Exists(modsPath))
                {
                    Program.Configuration.ModsPath = modsPath;
                    ConfigurationFile.Save();
                    WriteAndClear("New specified path accepted!", ConsoleColor.Green);
                }
                else
                {
                    WriteAndClear("Whoops! The specified path does not exist! Please enter a valid directory.");
                    continue;
                }

                break;
            }
        }

        public static void InitializeConsoleOptions()
        {
            Program.DefaultOptions = new ConsoleOptions("Pick any option:", new ListModsOption(), new ListExtractedModsOption(), new ListEnabledModsOption(), new UnpackModOption(), new DecompileModOption())
            {
                DisplayReturn = false,
                DisplayGoBack = false
            };

            ConsoleAPI.SelectedOptionSet = Program.DefaultOptions;
        }

        public static void InitializeProgramOptions() => Program.Configuration = ConfigurationFile.Load(Program.EXEPath + Path.DirectorySeparatorChar + "configuration.json");
    }
}
