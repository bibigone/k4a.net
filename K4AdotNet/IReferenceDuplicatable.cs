using System;
using System.Collections.Generic;
using System.Text;

namespace K4AdotNet
{
    public interface IReferenceDuplicatable<T>
        where T: class, IReferenceDuplicatable<T>
    {
        T DuplicateReference();
    }
}
