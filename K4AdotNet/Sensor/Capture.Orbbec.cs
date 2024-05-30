using System;
using System.Diagnostics;

namespace K4AdotNet.Sensor
{
    partial class Capture
    {
        /// <summary>
        /// Implementation of base <see cref="Capture"/> class for Orbbec Femto devices.
        /// This class works via `Orbbec SDK K4A Wrapper` native libraries.
        /// </summary>
        /// <remarks>Supported in modes <see cref="ComboMode.Orbbec"/> and <see cref="ComboMode.Both"/>.</remarks>
        public class Orbbec : Capture
        {
            /// <summary>Creates an empty capture object for Orbbec implementation.</summary>
            /// <exception cref="InvalidOperationException">
            /// Sensor SDK fails to create empty capture object for some reason.
            /// Or the library is not initialized.
            /// Or the library is initialized in incompatible mode.
            /// </exception>
            public Orbbec()
                : base(CreateCaptureHandle(NativeApi.Orbbec.Instance))
            { }

            internal Orbbec(NativeHandles.CaptureHandle handle)
                : base(handle)
            {
                Debug.Assert(handle is NativeHandles.CaptureHandle.Orbbec);
            }

            /// <inheritdoc cref="Capture.DuplicateReference"/>
            public override Capture DuplicateReference()
                => new Orbbec(handle.ValueNotDisposed.DuplicateReference());

            /// <summary>
            /// Minor overriding of <see cref="Capture.ColorImage"/> property:
            /// Orbbec does not support "clearing" of <see cref="Capture.ColorImage"/> (setting value to <see langword="null"/> when current value is not <see langword="null"/>).
            /// </summary>
            /// <exception cref="NotSupportedException">
            /// Orbbec does not support "clearing" of <see cref="Capture.ColorImage"/> (setting value to <see langword="null"/> when current value is not <see langword="null"/>).
            /// </exception>
            public override Image? ColorImage
            {
                get => base.ColorImage;

                set
                {
                    if (value is null)
                    {
                        using var currentValue = base.ColorImage;
                        if (currentValue is null)
                            return;     // Noting to do
                        ThrowClearingOfPropertyValueIsNotSupported(nameof(ColorImage));
                    }

                    base.ColorImage = value;
                }
            }

            /// <summary>
            /// Minor overriding of <see cref="Capture.DepthImage"/> property:
            /// Orbbec does not support "clearing" of <see cref="Capture.DepthImage"/> (setting value to <see langword="null"/> when current value is not <see langword="null"/>).
            /// </summary>
            /// <exception cref="NotSupportedException">
            /// Orbbec does not support "clearing" of <see cref="Capture.DepthImage"/> (setting value to <see langword="null"/> when current value is not <see langword="null"/>).
            /// </exception>
            public override Image? DepthImage
            {
                get => base.DepthImage;

                set
                {
                    if (value is null)
                    {
                        using var currentValue = base.DepthImage;
                        if (currentValue is null)
                            return;     // Noting to do
                        ThrowClearingOfPropertyValueIsNotSupported(nameof(DepthImage));
                    }

                    base.DepthImage = value;
                }
            }

            /// <summary>
            /// Minor overriding of <see cref="Capture.IRImage"/> property:
            /// Orbbec does not support "clearing" of <see cref="Capture.IRImage"/> (setting value to <see langword="null"/> when current value is not <see langword="null"/>).
            /// </summary>
            /// <exception cref="NotSupportedException">
            /// Orbbec does not support "clearing" of <see cref="Capture.IRImage"/> (setting value to <see langword="null"/> when current value is not <see langword="null"/>).
            /// </exception>
            public override Image? IRImage
            {
                get => base.IRImage;

                set
                {
                    if (value is null)
                    {
                        using var currentValue = base.IRImage;
                        if (currentValue is null)
                            return;     // Noting to do
                        ThrowClearingOfPropertyValueIsNotSupported(nameof(IRImage));
                    }

                    base.IRImage = value;
                }
            }

            private void ThrowClearingOfPropertyValueIsNotSupported(string propName)
                => throw new NotSupportedException($"Orbbec does not support setting of {propName} property to null when current value is not null.");
        }
    }
}
