using CommandLine;
using K4AdotNet.Sensor;
using System;
using System.Diagnostics;

using static System.Console;

namespace K4AdotNet.Samples.Console.Recorder
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Sdk.ConfigureLogging(TraceLevel.Warning, logToStdout: true);
            Sdk.ConfigureRecordLogging(TraceLevel.Warning, logToStdout: true);

            Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        private static void Run(Options options)
        {
            try
            {
                var deviceIndex = options.GetDeviceIndex();
                var config = options.GetConfiguration();
                var dstFilePath = options.GetOutputFilePath();
                var lengthSeconds = options.RecordLengthSeconds;

                using (var device = OpenDevice(deviceIndex))
                {
                    WriteLine("Starting cameras...");
                    device.StartCameras(config);

                    WriteLine($"Creating video recorder to {dstFilePath}...");
                    using (var recorder = new Record.Recorder(dstFilePath, device, config))
                    {
                        recorder.WriteHeader();

                        WriteLine("Recording... (press any key to stop)");
                        var stopwatch = Stopwatch.StartNew();
                        while (!KeyAvailable && (lengthSeconds <= 0 || stopwatch.Elapsed.TotalSeconds < lengthSeconds))
                        {
                            Write(new string('\b', 64));
                            Write("{0:0.00} sec", stopwatch.Elapsed.TotalSeconds);
                            if (lengthSeconds > 0)
                                Write(" of {0} sec", lengthSeconds);
                            using (var capture = device.GetCapture())
                            {
                                if (capture != null)
                                {
                                    recorder.WriteCapture(capture);
                                }
                            }
                        }
                        if (KeyAvailable)
                            ReadKey(intercept: true);
                        WriteLine();
                    }
                }

                WriteLine("Done!");
            }
            catch (Exception exc)
            {
                WriteLine();
                System.Console.Error.WriteLine("ERROR:");
                System.Console.Error.WriteLine(exc.Message);
            }
        }

        private static Device OpenDevice(int deviceIndex)
        {
            var deviceName = "Azure Kinect device";
            if (deviceIndex != 0)
                deviceName += " #" + deviceIndex;
            WriteLine($"Connecting to {deviceName}...");
            if (!Device.TryOpen(out var device, index: deviceIndex))
                throw new ApplicationException($"Azure {deviceName} is not connected or is occupied by other software");
            return device;
        }
    }
}
