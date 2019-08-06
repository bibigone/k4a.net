using System;

namespace K4AdotNet
{
    /// <summary>
    /// Interface for objects which reference unmanaged objects and support reference counting.
    /// </summary>
    /// <typeparam name="T">Type implemented this interface.</typeparam>
    public interface IReferenceDuplicatable<T>
        where T: class, IReferenceDuplicatable<T>
    {
        /// <summary>
        /// Creates new managed object that references exactly the same unmanaged object as original one.
        /// </summary>
        /// <returns>
        /// New managed object that references exactly to the same unmanaged object as original one. Not <see langword="null"/>.
        /// </returns>
        /// <remarks><para>
        /// Under the hood, references counter is incremented during call to this method.
        /// </para><para>
        /// To release reference, that is decrement reference counter call <see cref="IDisposable.Dispose"/> method of object.
        /// </para><para>
        /// It helps to manage underlying object lifetime and to access object data from different threads and different components of application.
        /// </para></remarks>
        /// <exception cref="ObjectDisposedException">This method cannot be called for disposed objects.</exception>
        /// <seealso cref="IDisposable.Dispose"/>
        T DuplicateReference();
    }
}
