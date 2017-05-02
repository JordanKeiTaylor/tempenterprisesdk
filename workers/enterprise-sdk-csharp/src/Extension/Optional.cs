
using Improbable.Collections;

namespace EnterpriseSDK.Extension
{
    public static class Optional
    {
        public static Option<T> None<T>()
        {
            return new Option<T>();
        }
    }
}