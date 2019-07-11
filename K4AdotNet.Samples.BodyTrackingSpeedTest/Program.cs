using K4AdotNet.Sensor;
using System;
using System.Diagnostics;

namespace K4AdotNet.Samples.BodyTrackingSpeedTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Body tracking speed test on prerecorded video from Kinect for Azure device");

            var executionParameters = args.Length == 0
                ? AskExecutionParameters()
                : ParseCommandLineArguments(args);

            if (executionParameters == null)
            {
                PrintHowToUse();
                return;
            }

            Process(executionParameters);

            if (args.Length == 0)
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }

        private static void PrintProcessingStatus(Processor processor)
        {
            Console.Write(new string('\b', 50));
            Console.Write(new string(' ', 50));
            Console.Write(new string('\b', 50));
            Console.Write($"processed: {processor.TotalFrameCount}    with body: {processor.FrameWithBodyCount}    in buffer: {processor.QueueSize}");
        }

        private delegate bool ParameterSetter(string value, out string message);

        #region Asking parameters from STDIN

        private static ExecutionParameters AskExecutionParameters()
        {
            Console.WriteLine("No command line arguments specified.");
            Console.WriteLine("Please enter execution parameters:");
            var parameters = new ExecutionParameters();
            if (!AskParameter(ExecutionParameters.MkvPathDescription, parameters.TrySetMkvPath))
                return null;
            if (!AskParameter(ExecutionParameters.MultiThreadedDescription, parameters.TrySetMultiThreaded))
                return null;
            if (!AskParameter(ExecutionParameters.StartTimeDescription, parameters.TrySetStartTime))
                return null;
            if (!AskParameter(ExecutionParameters.EndTimeDescription, parameters.TrySetEndTime))
                return null;
            return parameters;
        }

        private const int MAX_ASK_ATTEMPTS = 3;

        private static bool AskParameter(string prompt, ParameterSetter setter)
        {
            for (var attempt = 0; attempt < MAX_ASK_ATTEMPTS; attempt++)
            {
                Console.Write(prompt+ ": ");
                var str = Console.ReadLine();
                if (setter(str, out var message))
                    return true;
                Console.WriteLine(message);
                Console.Beep();
            }
            return false;
        }

        #endregion

        #region Parsing parameters from command-line arguments

        private static ExecutionParameters ParseCommandLineArguments(string[] args)
        {
            var parameters = new ExecutionParameters();

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i].Trim();
                switch (arg.ToLowerInvariant())
                {
                    case "-t":
                    case "--threading":
                        if (!ParseArgument(args, ref i, parameters.TrySetMultiThreaded))
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
                            Console.WriteLine($"Invalid command-line argument \"{arg}\":");
                            if (arg.StartsWith("-") && !ExecutionParameters.IsValueLikeToMkvFilePath(arg))
                                message = "Unknown option " + arg;
                            Console.WriteLine(message);
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

            string message = null;
            if (value != null && setter(value, out message))
            {
                argIndex++;
                return true;
            }

            if (value == null || value.StartsWith("-") || ExecutionParameters.IsValueLikeToMkvFilePath(value))
            {
                Console.WriteLine("Invalid command-line arguments:");
                Console.WriteLine($"Option {name} must be followed by its value");
                return false;
            }

            argIndex++;
            Console.WriteLine($"Invalid value \"{value}\" of command-line option {name}:");
            Console.WriteLine(message);
            return false;
        }

        #endregion

        #region How to use

        private static void PrintHowToUse()
        {
            Console.WriteLine();
            Console.WriteLine("Usage: ");
            Console.WriteLine("dotnet K4AdotNet.Samples.BodyTrackingSpeedTest.dll [options] <mkvFile>");
            Console.WriteLine("where: ");
            Console.WriteLine("  <mkvFile> - " + ExecutionParameters.MkvPathDescription);
            Console.WriteLine("  options:");
            Console.WriteLine("    -t, --threading s|m\t\t" + ExecutionParameters.StartTimeDescription);
            Console.WriteLine("    -s, --startTime <time>\t\t" + ExecutionParameters.StartTimeDescription);
            Console.WriteLine("    -e, --endTime <time>\t\t" + ExecutionParameters.EndTimeDescription);
            Console.WriteLine();
            Console.WriteLine("When no command-line arguments specified, STDIN and STDOUT are used to ask parameters");
            Console.WriteLine();
        }

        #endregion

        #region Processing

        private static void Process(ExecutionParameters executionParameters)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("opening recording and creating body tracking pipeline...");
                using (var processor = Processor.Create(executionParameters))
                {
                    Console.WriteLine("opened:");
                    Console.WriteLine("  depth mode = " + processor.RecordConfig.DepthMode);
                    Console.WriteLine("  camera frame rate = " + processor.RecordConfig.CameraFps.ToNumberHz());
                    Console.WriteLine("  record length = " + processor.RecordLength);
                    Console.WriteLine("processing frames:");
                    var sw = Stopwatch.StartNew();
                    while (processor.NextFrame())
                    {
                        PrintProcessingStatus(processor);
                    }
                    sw.Stop();
                    PrintProcessingStatus(processor);
                    Console.WriteLine();
                    Console.WriteLine("done!");
                    if (sw.Elapsed.TotalSeconds > 0)
                    {
                        var trackingSpeed = processor.TotalFrameCount / sw.Elapsed.TotalSeconds;
                        Console.WriteLine($"tracking speed = {trackingSpeed} FPS");
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR!");
                Console.WriteLine(exc.ToString());
            }
        }

        #endregion
    }
}
