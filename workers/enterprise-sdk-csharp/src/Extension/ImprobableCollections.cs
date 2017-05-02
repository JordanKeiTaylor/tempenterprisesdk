using System.Collections.Generic;

namespace EnterpriseSDK.Extension
{
    public static class ImprobableCollections
    {
        public static Improbable.Collections.List<TSource> ToImprobableList<TSource>(this IEnumerable<TSource> source)
        {
            return new Improbable.Collections.List<TSource>(source);
        }
    }
}
