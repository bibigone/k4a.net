namespace K4AdotNet
{
    /// <summary>
    /// Base class for all objects that depend on the native implementation
    /// (`original K4A` or `Orbbec SDK K4A Wrapper`).
    /// </summary>
    /// <seealso cref="ComboMode"/>
    public abstract class SdkObject
    {
        /// <summary>Initializes properties <see cref="IsAzure"/> and <see cref="IsOrbbec"/>.</summary>
        /// <param name="isOrbbec">Value for <see cref="IsOrbbec"/> property.</param>
        /// <exception cref="System.InvalidOperationException">
        /// The library was not initialized or initialized in incompatible mode.
        /// </exception>
        protected SdkObject(bool isOrbbec)
        {
            IsOrbbec = isOrbbec;

            if (IsOrbbec)
                Sdk.Orbbec.CheckEnabled();
            else
                Sdk.Azure.CheckEnabled();
        }

        /// <summary>Does this object operate via `Orbbec SDK K4A Wrapper` native library under the hood?</summary>
        /// <seealso cref="ComboMode"/>
        public bool IsOrbbec { get; }

        /// <summary>Does this object operates via `original K4A` native library under the hood?</summary>
        /// <seealso cref="ComboMode"/>
        public bool IsAzure => !IsOrbbec;
    }
}
