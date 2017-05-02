using System.Collections.Generic;

namespace Improbable.Enterprise.Extension
{
    public static class ImprobableCollections
    {
        public static Improbable.Collections.List<TSource> ToImprobableList<TSource>(this IEnumerable<TSource> source)
        {
            return new Improbable.Collections.List<TSource>(source);
        }
    }
}
