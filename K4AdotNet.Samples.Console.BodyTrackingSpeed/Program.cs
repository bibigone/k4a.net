using K4AdotNet.Sensor;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using static System.Console;

namespace K4AdotNet.Samples.Console.BodyTrackingSpeed
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Sdk.ConfigureLogging(TraceLevel.Warning, logToStdout: true);
            Sdk.ConfigureRecordLogging(TraceLevel.Warning, logToStdout: true);
            Sdk.ConfigureBodyTrackingLogging(TraceLevel.Warning, logToStdout: true);

            WriteLine("Body tracking speed test on prerecorded video from Kinect for Azure device");

            var processingParameters = args.Length == 0
                ? AskProcessingParameters()
                : ParseCommandLineArguments(args);

            if (processingParameters == null)
            {
                PrintHowToUse();
                return;
            }

            WriteLine();
            Process(processingParameters);

            if (args.Length == 0)
            {
                WriteLine("Press any key to exit");
                ReadKey();
            }
        }

        private static void PrintProcessingStatus(Processor processor)
        {
            Write(new string('\b', 50));
            Write(new string(' ', 50));
            Write(new string('\b', 50));
            Write($"processed: {processor.TotalFrameCount}    with body: {processor.FrameWithBodyCount}    in buffer: {processor.QueueSize}");
        }

        private delegate bool ParameterSetter(string? value, [NotNullWhen(returnValue: false)] out string? message);

        #region Asking parameters from STDIN

        private static ProcessingParameters? AskProcessingParameters()
        {
            WriteLine("No command line arguments specified.");
            WriteLine("Please enter execution parameters:");
            var parameters = new ProcessingParameters();
            if (!AskParameter(ProcessingParameters.MkvPathDescription, parameters.TrySetMkvPath))
                return null;
            if (!AskParameter(ProcessingParameters.ProcessingModeDescription, parameters.TrySetProcessingMode))
                return null;
            if (!AskParameter(ProcessingParameters.DnnModelDescription, parameters.TrySetDnnModel))
                return null;
            if (!AskParameter(ProcessingParameters.ImplementationDescription, parameters.TrySetImplementation))
                return null;
            if (!AskParameter(ProcessingParameters.StartTimeDescription, parameters.TrySetStartTime))
                return null;
            if (!AskParameter(ProcessingParameters.EndTimeDescription, parameters.TrySetEndTime))
                return null;
            return parameters;
        }

        private const int MAX_ASK_ATTEMPTS = 3;

        private static bool AskParameter(string prompt, ParameterSetter setter)
        {
            for (var attempt = 0; attempt < MAX_ASK_ATTEMPTS; attempt++)
            {
                Write(prompt+ ": ");
                var str = ReadLine();
                if (setter(str, out var message))
                    return true;
                WriteLine(message);
                Beep();
            }
            return false;
        }

        #endregion

        #region Parsing parameters from command-line arguments

        private static ProcessingParameters? ParseCommandLineArguments(string[] args)
        {
            var parameters = new ProcessingParameters();

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i].Trim();
                switch (arg.ToLowerInvariant())
                {
                    case "-m":
                    case "--mode":
                        if (!ParseArgument(args, ref i, parameters.TrySetProcessingMode))
                            return null;
                        break;
                    case "-d":
                    case "--dnnModel":
                        if (!ParseArgument(args, ref i, parameters.TrySetDnnModel))
                            return null;
                        break;
                    case "-i":
                    case "--implementation":
                        if (!ParseArgument(args, ref i, parameters.TrySetImplementation))
                            return null;
                        break;
                    case "-s":
                    case "--startTime":
                        if (!ParseArgument(args, ref i, parameters.TrySetStartTime))
                            return null;
                        break;
                    case "-e":
                    case "--endTime":
                        if (!ParseArgument(args, ref i, parameters.TrySetEndTime))
                            return null;
                        break;
                    default:
                        if (!parameters.TrySetMkvPath(arg, out var message))
                        {
                            WriteLine($"Invalid command-line argument \"{arg}\":");
                            if (arg.StartsWith("-") && !ProcessingParameters.IsValueLikeToMkvFilePath(arg))
                                message = "Unknown option " + arg;
                            WriteLine(message);
                            return null;
                        }
                        break;
                }
            }

            return parameters;
        }

        private static bool ParseArgument(string[] args, ref int argIndex, ParameterSetter setter)
        {
            var name = args[argIndex];
            var value = argIndex + 1 < args.Length ? args[argIndex + 1] : null;

            string? message = null;
            if (value != null && setter(value, out message))
            {
                argIndex++;
                return true;
            }

            if (value == null || value.StartsWith("-") || ProcessingParameters.IsValueLikeToMkvFilePath(value))
            {
                WriteLine("Invalid command-line arguments:");
                WriteLine($"Option {name} must be followed by its value");
                return false;
            }

            argIndex++;
            WriteLine($"Invalid value \"{value}\" of command-line option {name}:");
            WriteLine(message);
            return false;
        }

        #endregion

        #region How to use

        private static void PrintHowToUse()
        {
            WriteLine();
            WriteLine("Usage: ");
            WriteLine("dotnet K4AdotNet.Samples.BodyTrackingSpeedTest.dll [options] <mkvFile>");
            WriteLine("where: ");
            WriteLine("  <mkvFile> - " + ProcessingParameters.MkvPathDescription);
            WriteLine("  options:");
            WriteLine("    -m, --mode c|g|u|t|d\t\t" + ProcessingParameters.ProcessingModeDescription);
            WriteLine("    -d, --dnnMode d|l\t\t" + ProcessingParameters.DnnModelDescription);
            WriteLine("    -i, --implementation s|p|e\t\t" + ProcessingParameters.ImplementationDescription);
            WriteLine("    -s, --startTime <time>\t\t" + ProcessingParameters.StartTimeDescription);
            WriteLine("    -e, --endTime <time>\t\t" + ProcessingParameters.EndTimeDescription);
            WriteLine();
            WriteLine("When no command-line arguments specified, STDIN and STDOUT are used to ask parameters");
            WriteLine();
        }

        #endregion

        #region Processing

        private static void Process(ProcessingParameters processingParameters)
        {
            try
            {
                WriteLine();
                WriteLine("opening recording and creating body tracking pipeline...");
                using (var processor = Processor.Create(processingParameters))
                {
                    WriteLine("opened:");
                    WriteLine("  depth mode = " + processor.RecordConfig.DepthMode);
                    WriteLine("  camera frame rate = " + processor.RecordConfig.CameraFps.ToNumberHz());
                    WriteLine("  record length = " + processor.RecordLength);
                    WriteLine("processing frames:");
                    var sw = Stopwatch.StartNew();
                    while (processor.NextFrame())
                    {
                        PrintProcessingStatus(processor);
                    }
                    sw.Stop();
                    PrintProcessingStatus(processor);
                    WriteLine();
                    WriteLine("done!");
                    if (sw.Elapsed.TotalSeconds > 0)
                    {
                        var trackingSpeed = processor.TotalFrameCount / sw.Elapsed.TotalSeconds;
                        WriteLine($"tracking speed = {trackingSpeed} FPS");
                    }
                }
            }
            catch (Exception exc)
            {
                WriteLine();
                WriteLine("ERROR!");
                WriteLine(exc.ToString());
            }
        }

        #endregion
    }
}
