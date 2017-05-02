
using Improbable.Collections;

namespace Improbable.Enterprise.Extension
{
    public static class Optional
    {
        public static Option<T> None<T>()
        {
            return new Option<T>();
        }
    }
}