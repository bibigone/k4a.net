using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace K4AdotNet.Samples.Console.BodyTrackingSpeed
{
    internal sealed class ProcessingParameters
    {
        public const string MKV_FILE_EXTENSION = ".mkv";

        public static readonly string MkvPathDescription = "Path to MKV file";
        public static readonly string ProcessingModeDescription = "Processing mode (C - CPU, G - GPU, U - CUDA, T - TensorRT, D - DirectML, default - C)";
        public static readonly string DnnModelDescription = "DNN model (D - Default, L - Lite, default - D)";
        public static readonly string ImplementationDescription = "Optional implementation type (S - single thread, P - pop in background, E - enqueue in background, default - S)";
        public static readonly string StartTimeDescription = "Optional start time of video interval in seconds (default - beginning of recording)";
        public static readonly string EndTimeDescription = "Optional end time of video interval in seconds (default - end of recording)";

        public static bool IsValueLikeToMkvFilePath(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            value = value.Trim();
            if (value.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return false;
            var extension = Path.GetExtension(value);
            if (!MKV_FILE_EXTENSION.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                return false;
            return true;
        }

        public string? MkvPath { get; private set; }
        public BodyTracking.TrackerProcessingMode ProcessingMode { get; private set; }
        public DnnModel DnnModel { get; private set; }
        public ProcessingImplementation Implementation { get; private set; }
        public TimeSpan? StartTime { get; private set; }
        public TimeSpan? EndTime { get; private set; }

        public bool IsTimeInStartEndInterval(TimeSpan frameTimestamp)
            => (!StartTime.HasValue || StartTime.Value <= frameTimestamp)
            && (!EndTime.HasValue || EndTime.Value >= frameTimestamp);

        public bool TrySetMkvPath(string? value, [NotNullWhen(returnValue: false)] out string? message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                message = "Path to MKV file cannot be empty";
                return false;
            }

            value = value.Trim();

            if (value.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                message = "Path to MKV file contains some invalid symbols";
                return false;
            }

            if (!File.Exists(value))
            {
                message = "MKV file does not exist.";
                return false;
            }

            var extension = Path.GetExtension(value);
            if (!MKV_FILE_EXTENSION.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
            {
                message = "Path to MKV video is expected";
                return false;
            }

            MkvPath = value;
            message = null;
            return true;
        }

        private static readonly IReadOnlyDictionary<string, BodyTracking.TrackerProcessingMode> processingModes
            = new Dictionary<string, BodyTracking.TrackerProcessingMode>
            {
                ["c"] = BodyTracking.TrackerProcessingMode.Cpu,
                ["g"] = BodyTracking.TrackerProcessingMode.Gpu,
                ["u"] = BodyTracking.TrackerProcessingMode.GpuCuda,
                ["t"] = BodyTracking.TrackerProcessingMode.GpuTensorRT,
                ["d"] = BodyTracking.TrackerProcessingMode.GpuDirectML,
            };

        public bool TrySetProcessingMode(string? value, [NotNullWhen(returnValue: false)] out string? message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                ProcessingMode = BodyTracking.TrackerProcessingMode.Cpu;
                message = null;
                return true;
            }

            value = value.Trim().ToLowerInvariant();
            if (processingModes.TryGetValue(value, out var mode))
            {
                ProcessingMode = mode;
                message = null;
                return true;
            }

            message = InvalidValueMessage(processingModes.Keys);
            return false;
        }

        private static string InvalidValueMessage(IEnumerable<string> possibleValues)
            => "Invalid value. Expected " + string.Join(" or ", possibleValues.Select(s => "'" + s.ToUpperInvariant() + "'")) + " characters.";

        private static readonly IReadOnlyDictionary<string, DnnModel> dnnModels
            = new Dictionary<string, DnnModel>
            {
                ["d"] = DnnModel.Default,
                ["l"] = DnnModel.Lite,
            };

        public bool TrySetDnnModel(string? value, [NotNullWhen(returnValue: false)] out string? message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                DnnModel = DnnModel.Default;
                message = null;
                return true;
            }

            value = value.Trim().ToLowerInvariant();
            if (dnnModels.TryGetValue(value, out var model))
            {
                DnnModel = model;
                message = null;
                return true;
            }

            message = InvalidValueMessage(dnnModels.Keys);
            return false;
        }

        private static readonly IReadOnlyDictionary<string, ProcessingImplementation> implementations
            = new Dictionary<string, ProcessingImplementation>
            {
                ["s"] = ProcessingImplementation.SingleThread,
                ["p"] = ProcessingImplementation.PopInBackground,
                ["e"] = ProcessingImplementation.EnqueueInBackground,
            };

        public bool TrySetImplementation(string? value, [NotNullWhen(returnValue: false)] out string? message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Implementation = ProcessingImplementation.SingleThread;
                message = null;
                return true;
            }

            value = value.Trim().ToLowerInvariant();
            if (implementations.TryGetValue(value, out var impl))
            {
                Implementation = impl;
                message = null;
                return true;
            }

            message = InvalidValueMessage(implementations.Keys);
            return false;
        }

        public bool TrySetStartTime(string? value, [NotNullWhen(returnValue: false)] out string? message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                StartTime = null;
                message = null;
                return true;
            }

            value = value.Trim();

            if (!double.TryParse(value, out var startTime))
            {
                message = "Start time must be a number in seconds";
                return false;
            }

            if (EndTime.HasValue && startTime > EndTime.Value.TotalSeconds)
            {
                message = "Start time cannot be greater than End time";
                return false;
            }

            StartTime = TimeSpan.FromSeconds(startTime);
            message = null;
            return true;
        }

        public bool TrySetEndTime(string? value, [NotNullWhen(returnValue: false)] out string? message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                EndTime = null;
                message = null;
                return true;
            }

            value = value.Trim();

            if (!double.TryParse(value, out var endTime))
            {
                message = "End time must be a number in seconds";
                return false;
            }

            if (StartTime.HasValue && endTime < StartTime.Value.TotalSeconds)
            {
                message = "End time cannot be less than Start time";
                return false;
            }

            EndTime = TimeSpan.FromSeconds(endTime);
            message = null;
            return true;
        }
    }
}
