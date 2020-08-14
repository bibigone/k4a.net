using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace K4AdotNet.Samples.Core.BodyTrackingSpeed
{
    internal sealed class ProcessingParameters
    {
        public const string MKV_FILE_EXTENSION = ".mkv";

        public static readonly string MkvPathDescription = "Path to MKV file";
        public static readonly string CpuOnlyModeDescription = "CPU/GPU mode (C - use only CPU, G - use GPU, default - G)";
        public static readonly string ImplementationDescription = "Optional implementation type (S - single thread, P - pop in background, E - enqueue in background, default - S)";
        public static readonly string StartTimeDescription = "Optional start time of video interval in seconds (default - beginning of recording)";
        public static readonly string EndTimeDescription = "Optional end time of video interval in seconds (default - end of recording)";

        public static bool IsValueLikeToMkvFilePath(string value)
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
        public bool CpuOnlyMode { get; private set; }
        public ProcessingImplementation Implementation { get; private set; }
        public TimeSpan? StartTime { get; private set; }
        public TimeSpan? EndTime { get; private set; }

        public bool IsTimeInStartEndInterval(TimeSpan frameTimestamp)
            => (!StartTime.HasValue || StartTime.Value <= frameTimestamp)
            && (!EndTime.HasValue || EndTime.Value >= frameTimestamp);

        public bool TrySetMkvPath(string value, [NotNullWhen(returnValue: false)] out string? message)
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

        public bool TrySetCpuOnlyMode(string value, [NotNullWhen(returnValue: false)] out string? message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                CpuOnlyMode = false;
                message = null;
                return true;
            }

            value = value.Trim().ToLowerInvariant();
            switch (value)
            {
                case "c":
                    CpuOnlyMode = true;
                    message = null;
                    return true;
                case "g":
                    CpuOnlyMode = false;
                    message = null;
                    return true;
            }

            message = $"Invalid value. Expected 'C' or 'G'.";
            return false;
        }

        public bool TrySetImplementation(string value, [NotNullWhen(returnValue: false)] out string? message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Implementation = ProcessingImplementation.SingleThread;
                message = null;
                return true;
            }

            value = value.Trim().ToLowerInvariant();
            switch (value)
            {
                case "s":
                    Implementation = ProcessingImplementation.SingleThread;
                    message = null;
                    return true;
                case "p":
                    Implementation = ProcessingImplementation.PopInBackground;
                    message = null;
                    return true;
                case "e":
                    Implementation = ProcessingImplementation.EnqueueInBackground;
                    message = null;
                    return true;
            }

            message = $"Invalid value. Expected 'S' or 'P' or 'E' characters.";
            return false;
        }

        public bool TrySetStartTime(string value, [NotNullWhen(returnValue: false)] out string? message)
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

        public bool TrySetEndTime(string value, [NotNullWhen(returnValue: false)] out string? message)
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
