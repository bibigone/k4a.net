using System;

namespace K4AdotNet.Sensor
{
    // Inspired by <c>depthengine_helper</c> class from <c>k4a.hpp</c>
    //
    /// <summary>Avoid deep engine initialization failures when using multiple OpenGL contexts within user applications and SDKs. Only for ORBBEC.</summary>
    /// <remarks>This function only needs to be called when on the Linux platform</remarks>
    public sealed class DepthEngineHelper : SdkObject, IDisposablePlus
    {
        /// <summary>Creates depthengine helper (ORBBEC only).</summary>
        /// <returns>Created depthengine helper. Not <see langword="null"/>. To release, call <see cref="Dispose"/> method.</returns>
        /// <exception cref="InvalidOperationException">Cannot create depthengine helper for some internal reason. For error details see logs.</exception>
        public static DepthEngineHelper Create()
        {
            var res = NativeApi.Orbbec.Instance.DepthEngineHelperCreate(out var handle);
            if (res != NativeCallResults.Result.Succeeded || handle is null || handle.IsInvalid)
                throw new InvalidOperationException("Failed to create depthengine helper");
            return new DepthEngineHelper(handle);
        }

        private readonly NativeHandles.HandleWrapper<NativeHandles.DepthEngineHandle> handle;    // This class is an wrapper around this native handle

        private DepthEngineHelper(NativeHandles.DepthEngineHandle handle)
            : base(isOrbbec: true)
        {
            this.handle = handle;
            this.handle.Disposed += Handle_Disposed;
        }

        private void Handle_Disposed(object? sender, EventArgs e)
        {
            handle.Disposed -= Handle_Disposed;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Call this method to release depthengine helper and free all unmanaged resources associated with current instance.
        /// </summary>
        /// <seealso cref="Disposed"/>
        /// <seealso cref="IsDisposed"/>
        public void Dispose()
            => handle.Dispose();

        /// <summary>Gets a value indicating whether the object has been disposed of.</summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed => handle.IsDisposed;

        /// <summary>Raised on object disposing (only once).</summary>
        /// <seealso cref="Dispose"/>
        public event EventHandler? Disposed;
    }
}
