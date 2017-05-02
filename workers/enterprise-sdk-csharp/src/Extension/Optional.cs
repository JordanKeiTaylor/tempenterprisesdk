
using Improbable.Collections;

namespace WorkerAPIFacade.Extension
{
    public static class Optional
    {
        public static Option<T> None<T>()
        {
            return new Option<T>();
        }
    }
}